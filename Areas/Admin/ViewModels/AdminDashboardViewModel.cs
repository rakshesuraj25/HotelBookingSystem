using HotelBookingSystem.Models.Enums;

namespace HotelBookingSystem.Areas.Admin.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalRooms { get; set; }
    public int AvailableRooms { get; set; }
    public int TotalBookings { get; set; }
    public int PendingBookings { get; set; }
    public int ConfirmedBookings { get; set; }
    public int CancelledBookings { get; set; }
    public int CompletedBookings { get; set; }
    public int RegisteredGuests { get; set; }

    public IReadOnlyList<AdminBookingRowViewModel> RecentBookings { get; set; } = Array.Empty<AdminBookingRowViewModel>();
}

public class AdminBookingRowViewModel
{
    public int Id { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public string GuestName { get; set; } = string.Empty;
    public string GuestEmail { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalAmount { get; set; }
    public BookingStatus BookingStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}
