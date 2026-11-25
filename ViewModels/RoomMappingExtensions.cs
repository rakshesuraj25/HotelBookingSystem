using HotelBookingSystem.Models;

namespace HotelBookingSystem.ViewModels;

internal static class RoomMappingExtensions
{
    public static RoomCardViewModel ToCardViewModel(this Room room)
    {
        var description = string.IsNullOrWhiteSpace(room.Description)
            ? "Thoughtfully designed accommodation with modern comforts."
            : room.Description.Trim();

        var shortDescription = description.Length > 125
            ? $"{description[..122].TrimEnd()}..."
            : description;

        return new RoomCardViewModel
        {
            Id = room.Id,
            RoomNumber = room.RoomNumber,
            Name = room.Name,
            ShortDescription = shortDescription,
            RoomType = room.RoomType,
            PricePerNight = room.PricePerNight,
            Capacity = room.Capacity,
            BedType = room.BedType,
            ImageUrl = string.IsNullOrWhiteSpace(room.ImageUrl) ? "/images/room-placeholder.svg" : room.ImageUrl,
            Amenities = ParseAmenities(room.Amenities)
        };
    }

    public static IReadOnlyList<string> ParseAmenities(string? amenities)
    {
        if (string.IsNullOrWhiteSpace(amenities))
        {
            return Array.Empty<string>();
        }

        return amenities
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToArray();
    }
}
