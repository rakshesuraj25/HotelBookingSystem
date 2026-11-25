using HotelBookingSystem.Data;
using HotelBookingSystem.Interfaces;
using HotelBookingSystem.Models;
using HotelBookingSystem.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Services;

/// <summary>
/// Handles guest registration and credential verification using IPasswordHasherService.
/// </summary>
public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasherService _passwordHasher;

    public AuthService(ApplicationDbContext context, IPasswordHasherService passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResult> RegisterGuestAsync(string fullName, string email, string? phoneNumber, string password)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var exists = await _context.ApplicationUsers
            .AnyAsync(u => u.Email.ToLower() == normalizedEmail);

        if (exists)
        {
            return new AuthResult(false, "An account with this email address already exists.");
        }

        // Always enforce Guest role server-side during self-registration.
        var user = new ApplicationUser
        {
            FullName = fullName.Trim(),
            Email = normalizedEmail,
            PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim(),
            Role = UserRole.Guest,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, password);

        await _context.ApplicationUsers.AddAsync(user);
        await _context.SaveChangesAsync();

        return new AuthResult(true, null, user);
    }

    public async Task<AuthResult> ValidateLoginAsync(string email, string password)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var user = await _context.ApplicationUsers
            .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

        if (user is null)
        {
            return new AuthResult(false, "Invalid email or password.");
        }

        if (!user.IsActive)
        {
            return new AuthResult(false, "This account has been deactivated. Please contact hotel support.");
        }

        var isValid = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (!isValid)
        {
            return new AuthResult(false, "Invalid email or password.");
        }

        return new AuthResult(true, null, user);
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(int userId)
    {
        return await _context.ApplicationUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
}
