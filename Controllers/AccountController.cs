using System.Security.Claims;
using HotelBookingSystem.Interfaces;
using HotelBookingSystem.Models;
using HotelBookingSystem.Models.Enums;
using HotelBookingSystem.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly IBookingService _bookingService;

    public AccountController(IAuthService authService, IBookingService bookingService)
    {
        _authService = authService;
        _bookingService = bookingService;
    }

    [HttpGet]
    public IActionResult Register(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new RegisterViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authService.RegisterGuestAsync(
            model.FullName,
            model.Email,
            model.PhoneNumber,
            model.Password);

        if (!result.Succeeded || result.User is null)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Registration failed.");
            return View(model);
        }

        await SignInUserAsync(result.User, isPersistent: false);

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authService.ValidateLoginAsync(model.Email, model.Password);
        if (!result.Succeeded || result.User is null)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Invalid email or password.");
            return View(model);
        }

        await SignInUserAsync(result.User, model.RememberMe);

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        if (result.User.Role == UserRole.Admin)
        {
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var rawId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(rawId, out var userId))
        {
            return RedirectToAction(nameof(Login));
        }

        var user = await _authService.GetUserByIdAsync(userId);
        if (user is null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        var bookings = await _bookingService.GetUserBookingsAsync(userId);

        var viewModel = new ProfileViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            TotalBookings = bookings.Count
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private async Task SignInUserAsync(ApplicationUser user, bool isPersistent)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = isPersistent,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            authProperties);
    }
}
