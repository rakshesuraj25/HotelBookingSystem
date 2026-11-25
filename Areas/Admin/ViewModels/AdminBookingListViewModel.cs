using HotelBookingSystem.Models.Enums;

namespace HotelBookingSystem.Areas.Admin.ViewModels;

public class AdminBookingListViewModel
{
    public string? SearchReference { get; set; }
    public BookingStatus? StatusFilter { get; set; }
    public IReadOnlyList<AdminBookingRowViewModel> Bookings { get; set; } = Array.Empty<AdminBookingRowViewModel>();
}

public class AdminBookingDetailsViewModel
{
    public int Id { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public BookingStatus BookingStatus { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfNights { get; set; }
    public int NumberOfGuests { get; set; }
    public decimal PricePerNight { get; set; }
    public decimal TotalAmount { get; set; }
    public string? SpecialRequests { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public int GuestId { get; set; }
    public string GuestName { get; set; } = string.Empty;
    public string GuestEmail { get; set; } = string.Empty;
    public string? GuestPhone { get; set; }

    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public RoomType RoomType { get; set; }
}
