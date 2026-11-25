using HotelBookingSystem.Models.Enums;

namespace HotelBookingSystem.ViewModels;

public class RoomDetailsViewModel
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RoomType RoomType { get; set; }
    public decimal PricePerNight { get; set; }
    public int Capacity { get; set; }
    public string BedType { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = "/images/room-placeholder.svg";
    public IReadOnlyList<string> Amenities { get; set; } = Array.Empty<string>();

    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public int? Guests { get; set; }
}
