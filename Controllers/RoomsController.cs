using HotelBookingSystem.Interfaces;
using HotelBookingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.Controllers;

public class RoomsController : Controller
{
    private readonly IRoomService _roomService;
    private readonly ILogger<RoomsController> _logger;

    public RoomsController(IRoomService roomService, ILogger<RoomsController> logger)
    {
        _roomService = roomService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] RoomSearchViewModel filter)
    {
        var hasDateFilter = filter.CheckInDate.HasValue || filter.CheckOutDate.HasValue;
        var hasAnyFilter = hasDateFilter || filter.Guests.HasValue || filter.RoomType.HasValue;
        filter.HasSearched = hasAnyFilter;

        if (filter.CheckInDate.HasValue != filter.CheckOutDate.HasValue)
        {
            ModelState.AddModelError(string.Empty, "Please select both check-in and check-out dates to check availability.");
        }
        else if (filter.CheckInDate.HasValue && filter.CheckOutDate.HasValue)
        {
            var checkIn = filter.CheckInDate.Value.Date;
            var checkOut = filter.CheckOutDate.Value.Date;

            if (checkIn < DateTime.UtcNow.Date)
            {
                ModelState.AddModelError(nameof(filter.CheckInDate), "Check-in date cannot be in the past.");
            }
            else if (checkOut <= checkIn)
            {
                ModelState.AddModelError(nameof(filter.CheckOutDate), "Check-out date must be later than check-in date.");
            }
        }

        if (!ModelState.IsValid)
        {
            var fallbackRooms = await _roomService.GetAllRoomsAsync();
            filter.Rooms = fallbackRooms.Select(r => r.ToCardViewModel()).ToList();
            return View(filter);
        }

        try
        {
            var rooms = hasAnyFilter
                ? await _roomService.SearchAvailableRoomsAsync(
                    filter.CheckInDate,
                    filter.CheckOutDate,
                    filter.Guests,
                    filter.RoomType)
                : await _roomService.GetAllRoomsAsync();

            filter.Rooms = rooms.Select(r => r.ToCardViewModel()).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load rooms.");
            ModelState.AddModelError(string.Empty, "We could not load available rooms right now. Please try again shortly.");
            filter.Rooms = Array.Empty<RoomCardViewModel>();
        }

        return View(filter);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, DateTime? checkInDate, DateTime? checkOutDate, int? guests)
    {
        var room = await _roomService.GetRoomByIdAsync(id);
        if (room is null)
        {
            return NotFound();
        }

        var viewModel = new RoomDetailsViewModel
        {
            Id = room.Id,
            RoomNumber = room.RoomNumber,
            Name = room.Name,
            Description = string.IsNullOrWhiteSpace(room.Description)
                ? "Thoughtfully appointed room with modern comforts and attentive service."
                : room.Description,
            RoomType = room.RoomType,
            PricePerNight = room.PricePerNight,
            Capacity = room.Capacity,
            BedType = room.BedType,
            ImageUrl = string.IsNullOrWhiteSpace(room.ImageUrl) ? "/images/room-placeholder.svg" : room.ImageUrl,
            Amenities = RoomMappingExtensions.ParseAmenities(room.Amenities),
            CheckInDate = checkInDate?.Date,
            CheckOutDate = checkOutDate?.Date,
            Guests = guests
        };

        return View(viewModel);
    }
}
