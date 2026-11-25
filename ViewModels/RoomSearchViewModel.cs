using System.ComponentModel.DataAnnotations;
using HotelBookingSystem.Models.Enums;

namespace HotelBookingSystem.ViewModels;

public class RoomSearchViewModel
{
    [DataType(DataType.Date)]
    [Display(Name = "Check-in")]
    public DateTime? CheckInDate { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Check-out")]
    public DateTime? CheckOutDate { get; set; }

    [Range(1, 20, ErrorMessage = "Guests must be between 1 and 20.")]
    [Display(Name = "Guests")]
    public int? Guests { get; set; }

    [Display(Name = "Room Type")]
    public RoomType? RoomType { get; set; }

    public bool HasSearched { get; set; }

    public IReadOnlyList<RoomCardViewModel> Rooms { get; set; } = Array.Empty<RoomCardViewModel>();
}
