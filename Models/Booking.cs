using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HotelBookingSystem.Models.Enums;

namespace HotelBookingSystem.Models;

/// <summary>
/// Represents a room reservation made by a user, preserving the nightly rate at booking time.
/// </summary>
public class Booking : IValidatableObject
{
    public int Id { get; set; }

    [Required]
    [StringLength(30)]
    public string BookingReference { get; set; } = string.Empty;

    [Required]
    public int UserId { get; set; }

    [Required]
    public int RoomId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime CheckInDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime CheckOutDate { get; set; }

    [Required]
    [Range(1, 20, ErrorMessage = "Number of guests must be at least 1.")]
    public int NumberOfGuests { get; set; }

    [Required]
    [Range(1, 365, ErrorMessage = "Number of nights must be at least 1.")]
    public int NumberOfNights { get; set; }

    /// <summary>
    /// Snapshot of the room's nightly rate at the time the booking was created.
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    [Range(typeof(decimal), "0.01", "999999.99", ErrorMessage = "Price per night must be greater than zero.")]
    public decimal PricePerNight { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    [Range(typeof(decimal), "0.01", "99999999.99", ErrorMessage = "Total amount must be greater than zero.")]
    public decimal TotalAmount { get; set; }

    [Required]
    public BookingStatus BookingStatus { get; set; } = BookingStatus.Pending;

    [StringLength(500)]
    public string? SpecialRequests { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ApplicationUser User { get; set; } = null!;

    public Room Room { get; set; } = null!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CheckOutDate.Date <= CheckInDate.Date)
        {
            yield return new ValidationResult(
                "Check-out date must be later than check-in date.",
                new[] { nameof(CheckOutDate), nameof(CheckInDate) });
        }
    }
}
