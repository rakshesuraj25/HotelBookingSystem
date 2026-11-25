using HotelBookingSystem.Models.Enums;

namespace HotelBookingSystem.ViewModels;

public class BookingConfirmationViewModel
{
    public int BookingId { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public RoomType RoomType { get; set; }
    public string ImageUrl { get; set; } = "/images/room-placeholder.svg";
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }
    public int NumberOfNights { get; set; }
    public decimal PricePerNight { get; set; }
    public decimal TotalAmount { get; set; }
    public BookingStatus BookingStatus { get; set; }
    public string? SpecialRequests { get; set; }
    public DateTime CreatedAt { get; set; }
}
