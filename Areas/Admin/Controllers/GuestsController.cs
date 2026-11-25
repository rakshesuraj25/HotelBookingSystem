using HotelBookingSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class GuestsController : Controller
{
    private readonly IAdminService _adminService;

    public GuestsController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var guests = await _adminService.GetGuestsAsync();
        return View(guests);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var guest = await _adminService.GetGuestDetailsAsync(id);
        if (guest is null)
        {
            return NotFound();
        }

        return View(guest);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var result = await _adminService.ToggleGuestActiveAsync(id);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] =
            result.Succeeded ? "Guest account status updated." : (result.ErrorMessage ?? "Unable to update guest status.");

        return RedirectToAction(nameof(Index));
    }
}
