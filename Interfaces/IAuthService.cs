using HotelBookingSystem.Models;

namespace HotelBookingSystem.Interfaces;

public sealed record AuthResult(bool Succeeded, string? ErrorMessage = null, ApplicationUser? User = null);

public interface IAuthService
{
    Task<AuthResult> RegisterGuestAsync(string fullName, string email, string? phoneNumber, string password);

    Task<AuthResult> ValidateLoginAsync(string email, string password);

    Task<ApplicationUser?> GetUserByIdAsync(int userId);
}
