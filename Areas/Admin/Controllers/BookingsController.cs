using HotelBookingSystem.Areas.Admin.ViewModels;
using HotelBookingSystem.Interfaces;
using HotelBookingSystem.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class BookingsController : Controller
{
    private readonly IAdminService _adminService;
    private readonly IBookingService _bookingService;

    public BookingsController(IAdminService adminService, IBookingService bookingService)
    {
        _adminService = adminService;
        _bookingService = bookingService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchReference, BookingStatus? statusFilter)
    {
        var bookings = await _adminService.GetBookingsAsync(searchReference, statusFilter);

        var model = new AdminBookingListViewModel
        {
            SearchReference = searchReference,
            StatusFilter = statusFilter,
            Bookings = bookings
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var details = await _adminService.GetBookingDetailsAsync(id);
        if (details is null)
        {
            return NotFound();
        }

        return View(details);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, BookingStatus newStatus)
    {
        var result = await _bookingService.UpdateBookingStatusAsync(id, newStatus);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] =
            result.Succeeded ? $"Booking status updated to {newStatus}." : (result.ErrorMessage ?? "Unable to update booking status.");

        return RedirectToAction(nameof(Details), new { id });
    }
}
