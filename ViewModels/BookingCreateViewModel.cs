using System.ComponentModel.DataAnnotations;
using HotelBookingSystem.Models.Enums;

namespace HotelBookingSystem.ViewModels;

public class BookingCreateViewModel
{
    [Required]
    public int RoomId { get; set; }

    // Display-only summary fields populated on the server (never trusted from form posts).
    public string RoomName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public RoomType RoomType { get; set; }
    public string BedType { get; set; } = string.Empty;
    public int RoomCapacity { get; set; }
    public decimal PricePerNight { get; set; }
    public string ImageUrl { get; set; } = "/images/room-placeholder.svg";

    [Required(ErrorMessage = "Please select a check-in date.")]
    [DataType(DataType.Date)]
    [Display(Name = "Check-in Date")]
    public DateTime CheckInDate { get; set; }

    [Required(ErrorMessage = "Please select a check-out date.")]
    [DataType(DataType.Date)]
    [Display(Name = "Check-out Date")]
    public DateTime CheckOutDate { get; set; }

    [Required(ErrorMessage = "Please enter the number of guests.")]
    [Range(1, 20, ErrorMessage = "Number of guests must be at least 1.")]
    [Display(Name = "Number of Guests")]
    public int NumberOfGuests { get; set; } = 1;

    [StringLength(500, ErrorMessage = "Special requests cannot exceed 500 characters.")]
    [Display(Name = "Special Requests")]
    public string? SpecialRequests { get; set; }

    public int CalculatedNights =>
        CheckOutDate.Date > CheckInDate.Date
            ? (CheckOutDate.Date - CheckInDate.Date).Days
            : 0;

    public decimal EstimatedTotal => CalculatedNights * PricePerNight;

    public bool RequiresAuthenticationNotice { get; set; }
}
