using System.ComponentModel.DataAnnotations;
using HotelBookingSystem.Models.Enums;

namespace HotelBookingSystem.Models;

/// <summary>
/// Represents a registered user (guest or administrator) in the system.
/// </summary>
public class ApplicationUser
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Stores a securely hashed password (never plaintext).
    /// </summary>
    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Phone]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [Required]
    public UserRole Role { get; set; } = UserRole.Guest;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
