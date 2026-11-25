using HotelBookingSystem.Models;

namespace HotelBookingSystem.Interfaces;

/// <summary>
/// Provides secure password hashing and verification for application users.
/// </summary>
public interface IPasswordHasherService
{
    string HashPassword(ApplicationUser user, string password);

    bool VerifyHashedPassword(ApplicationUser user, string hashedPassword, string providedPassword);
}
