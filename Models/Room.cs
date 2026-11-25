using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HotelBookingSystem.Models.Enums;

namespace HotelBookingSystem.Models;

/// <summary>
/// Represents a hotel room available for reservation.
/// </summary>
public class Room
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string RoomNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public RoomType RoomType { get; set; } = RoomType.Standard;

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    [Range(typeof(decimal), "0.01", "999999.99", ErrorMessage = "Price per night must be greater than zero.")]
    public decimal PricePerNight { get; set; }

    [Required]
    [Range(1, 20, ErrorMessage = "Capacity must be at least 1 guest.")]
    public int Capacity { get; set; }

    [Required]
    [StringLength(50)]
    public string BedType { get; set; } = string.Empty;

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    [StringLength(500)]
    public string? Amenities { get; set; }

    [Required]
    public RoomStatus RoomStatus { get; set; } = RoomStatus.Available;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
