using HotelBookingSystem.Models.Enums;

namespace HotelBookingSystem.ViewModels;

public class ProfileViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TotalBookings { get; set; }
}
