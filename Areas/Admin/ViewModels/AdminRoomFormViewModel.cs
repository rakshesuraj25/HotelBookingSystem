using System.ComponentModel.DataAnnotations;
using HotelBookingSystem.Models.Enums;

namespace HotelBookingSystem.Areas.Admin.ViewModels;

public class AdminRoomFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Room number is required.")]
    [StringLength(20)]
    [Display(Name = "Room Number")]
    public string RoomNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Room name is required.")]
    [StringLength(100)]
    [Display(Name = "Room Name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [Display(Name = "Room Type")]
    public RoomType RoomType { get; set; } = RoomType.Standard;

    [Required]
    [Range(typeof(decimal), "0.01", "999999.99", ErrorMessage = "Price per night must be greater than zero.")]
    [Display(Name = "Price Per Night (₹)")]
    public decimal PricePerNight { get; set; }

    [Required]
    [Range(1, 20, ErrorMessage = "Capacity must be between 1 and 20 guests.")]
    public int Capacity { get; set; } = 2;

    [Required(ErrorMessage = "Bed type is required.")]
    [StringLength(50)]
    [Display(Name = "Bed Type")]
    public string BedType { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Image Path / URL")]
    public string? ImageUrl { get; set; }

    [StringLength(500)]
    [Display(Name = "Amenities (comma-separated)")]
    public string? Amenities { get; set; }

    [Required]
    [Display(Name = "Room Status")]
    public RoomStatus RoomStatus { get; set; } = RoomStatus.Available;
}
