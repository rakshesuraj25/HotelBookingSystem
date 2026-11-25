using System.Security.Claims;
using HotelBookingSystem.Interfaces;
using HotelBookingSystem.Models;
using HotelBookingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.Controllers;

[Authorize]
public class BookingsController : Controller
{
    private readonly IRoomService _roomService;
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(
        IRoomService roomService,
        IBookingService bookingService,
        ILogger<BookingsController> logger)
    {
        _roomService = roomService;
        _bookingService = bookingService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int roomId, DateTime? checkInDate, DateTime? checkOutDate, int? guests)
    {
        var room = await _roomService.GetRoomByIdAsync(roomId);
        if (room is null)
        {
            return NotFound();
        }

        var today = DateTime.UtcNow.Date;
        var defaultCheckIn = checkInDate?.Date ?? today.AddDays(1);
        if (defaultCheckIn < today)
        {
            defaultCheckIn = today.AddDays(1);
        }

        var defaultCheckOut = checkOutDate?.Date ?? defaultCheckIn.AddDays(1);
        if (defaultCheckOut <= defaultCheckIn)
        {
            defaultCheckOut = defaultCheckIn.AddDays(1);
        }

        var defaultGuests = Math.Clamp(guests ?? 1, 1, room.Capacity);

        var viewModel = new BookingCreateViewModel
        {
            CheckInDate = defaultCheckIn,
            CheckOutDate = defaultCheckOut,
            NumberOfGuests = defaultGuests,
            RequiresAuthenticationNotice = false
        };

        PopulateRoomSummary(viewModel, room);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookingCreateViewModel model)
    {
        var room = await _roomService.GetRoomByIdAsync(model.RoomId);
        if (room is null)
        {
            return NotFound();
        }

        // Always repopulate room pricing and metadata on the server; never trust client-posted prices.
        PopulateRoomSummary(model, room);

        if (model.CheckInDate.Date < DateTime.UtcNow.Date)
        {
            ModelState.AddModelError(nameof(model.CheckInDate), "Check-in date cannot be in the past.");
        }

        if (model.CheckOutDate.Date <= model.CheckInDate.Date)
        {
            ModelState.AddModelError(nameof(model.CheckOutDate), "Check-out date must be later than check-in date.");
        }

        if (model.NumberOfGuests > room.Capacity)
        {
            ModelState.AddModelError(
                nameof(model.NumberOfGuests),
                $"This room accommodates up to {room.Capacity} guest(s).");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = GetAuthenticatedUserId();
        if (userId is null)
        {
            return Challenge();
        }

        try
        {
            var result = await _bookingService.CreateBookingAsync(
                userId.Value,
                room.Id,
                model.CheckInDate,
                model.CheckOutDate,
                model.NumberOfGuests,
                model.SpecialRequests);

            if (!result.Succeeded || result.Booking is null)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Unable to complete your booking.");
                return View(model);
            }

            return RedirectToAction(nameof(Confirmation), new { id = result.Booking.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating booking for RoomId {RoomId}.", model.RoomId);
            ModelState.AddModelError(string.Empty, "An unexpected error occurred while creating your booking. Please try again.");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Confirmation(int id)
    {
        var booking = await GetOwnedBookingOrNullAsync(id);
        if (booking is null)
        {
            return NotFound();
        }

        if (!IsBookingAccessible(booking))
        {
            return Forbid();
        }

        return View(ToConfirmationViewModel(booking));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var booking = await GetOwnedBookingOrNullAsync(id);
        if (booking is null)
        {
            return NotFound();
        }

        if (!IsBookingAccessible(booking))
        {
            return Forbid();
        }

        return View(ToConfirmationViewModel(booking));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = GetAuthenticatedUserId();
        if (userId is null)
        {
            return Challenge();
        }

        var result = await _bookingService.CancelBookingAsync(id, userId.Value, isAdmin: User.IsInRole("Admin"));
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "Unable to cancel booking.";
        }
        else
        {
            TempData["SuccessMessage"] = "Your reservation has been cancelled.";
        }

        return RedirectToAction(nameof(MyBookings));
    }

    [HttpGet]
    public async Task<IActionResult> MyBookings()
    {
        var userId = GetAuthenticatedUserId();
        if (userId is null)
        {
            return Challenge();
        }

        ViewBag.RequiresAuthentication = false;
        var bookings = await _bookingService.GetUserBookingsAsync(userId.Value);
        return View(bookings);
    }

    private async Task<Booking?> GetOwnedBookingOrNullAsync(int id)
    {
        return await _bookingService.GetBookingByIdAsync(id);
    }

    private bool IsBookingAccessible(Booking booking)
    {
        var userId = GetAuthenticatedUserId();
        return userId is not null && (booking.UserId == userId.Value || User.IsInRole("Admin"));
    }

    private int? GetAuthenticatedUserId()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var rawId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(rawId, out var userId) ? userId : null;
    }

    private static BookingConfirmationViewModel ToConfirmationViewModel(Booking booking)
    {
        return new BookingConfirmationViewModel
        {
            BookingId = booking.Id,
            BookingReference = booking.BookingReference,
            RoomName = booking.Room.Name,
            RoomNumber = booking.Room.RoomNumber,
            RoomType = booking.Room.RoomType,
            ImageUrl = string.IsNullOrWhiteSpace(booking.Room.ImageUrl) ? "/images/room-placeholder.svg" : booking.Room.ImageUrl,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            NumberOfGuests = booking.NumberOfGuests,
            NumberOfNights = booking.NumberOfNights,
            PricePerNight = booking.PricePerNight,
            TotalAmount = booking.TotalAmount,
            BookingStatus = booking.BookingStatus,
            SpecialRequests = booking.SpecialRequests,
            CreatedAt = booking.CreatedAt
        };
    }

    private static void PopulateRoomSummary(BookingCreateViewModel model, Room room)
    {
        model.RoomId = room.Id;
        model.RoomName = room.Name;
        model.RoomNumber = room.RoomNumber;
        model.RoomType = room.RoomType;
        model.BedType = room.BedType;
        model.RoomCapacity = room.Capacity;
        model.PricePerNight = room.PricePerNight;
        model.ImageUrl = string.IsNullOrWhiteSpace(room.ImageUrl) ? "/images/room-placeholder.svg" : room.ImageUrl;
    }
}
