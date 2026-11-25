using HotelBookingSystem.Interfaces;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace HotelBookingSystem.Services;

/// <summary>
/// Wraps ASP.NET Core Identity's PBKDF2 PasswordHasher for ApplicationUser.
/// </summary>
public class PasswordHasherService : IPasswordHasherService
{
    private readonly PasswordHasher<ApplicationUser> _passwordHasher;

    public PasswordHasherService(PasswordHasher<ApplicationUser> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public string HashPassword(ApplicationUser user, string password)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        return _passwordHasher.HashPassword(user, password);
    }

    public bool VerifyHashedPassword(ApplicationUser user, string hashedPassword, string providedPassword)
    {
        if (user is null || string.IsNullOrWhiteSpace(hashedPassword) || string.IsNullOrEmpty(providedPassword))
        {
            return false;
        }

        var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
