using HotelBookingSystem.Areas.Admin.ViewModels;
using HotelBookingSystem.Interfaces;
using HotelBookingSystem.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class RoomsController : Controller
{
    private readonly IAdminService _adminService;

    public RoomsController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var rooms = await _adminService.GetAllRoomsAsync();
        return View(rooms);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new AdminRoomFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdminRoomFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _adminService.CreateRoomAsync(model);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Unable to create room.");
            return View(model);
        }

        TempData["SuccessMessage"] = $"Room {model.RoomNumber} created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var room = await _adminService.GetRoomByIdAsync(id);
        if (room is null)
        {
            return NotFound();
        }

        var model = new AdminRoomFormViewModel
        {
            Id = room.Id,
            RoomNumber = room.RoomNumber,
            Name = room.Name,
            Description = room.Description,
            RoomType = room.RoomType,
            PricePerNight = room.PricePerNight,
            Capacity = room.Capacity,
            BedType = room.BedType,
            ImageUrl = room.ImageUrl,
            Amenities = room.Amenities,
            RoomStatus = room.RoomStatus
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AdminRoomFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _adminService.UpdateRoomAsync(model);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Unable to update room.");
            return View(model);
        }

        TempData["SuccessMessage"] = $"Room {model.RoomNumber} updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(int id, RoomStatus status)
    {
        var result = await _adminService.ChangeRoomStatusAsync(id, status);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] =
            result.Succeeded ? "Room status updated." : (result.ErrorMessage ?? "Unable to update status.");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _adminService.DeleteRoomAsync(id);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] =
            result.Succeeded ? "Room deleted." : (result.ErrorMessage ?? "Unable to delete room.");

        return RedirectToAction(nameof(Index));
    }
}
