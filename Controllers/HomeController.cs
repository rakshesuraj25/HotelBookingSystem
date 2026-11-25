using System.Diagnostics;
using HotelBookingSystem.Interfaces;
using HotelBookingSystem.Models;
using HotelBookingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.Controllers;

public class HomeController : Controller
{
    private readonly IRoomService _roomService;

    public HomeController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    public async Task<IActionResult> Index()
    {
        var featuredRooms = await _roomService.GetFeaturedRoomsAsync(4);
        var today = DateTime.UtcNow.Date;

        var viewModel = new HomeIndexViewModel
        {
            Search = new RoomSearchViewModel
            {
                CheckInDate = today.AddDays(1),
                CheckOutDate = today.AddDays(2),
                Guests = 2
            },
            FeaturedRooms = featuredRooms.Select(r => r.ToCardViewModel()).ToList()
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
