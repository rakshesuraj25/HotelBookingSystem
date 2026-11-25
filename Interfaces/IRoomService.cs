using HotelBookingSystem.Models;
using HotelBookingSystem.Models.Enums;

namespace HotelBookingSystem.Interfaces;

/// <summary>
/// Provides read and availability search operations for hotel rooms.
/// </summary>
public interface IRoomService
{
    Task<IReadOnlyList<Room>> GetAllRoomsAsync();

    Task<Room?> GetRoomByIdAsync(int id);

    Task<IReadOnlyList<Room>> SearchAvailableRoomsAsync(
        DateTime? checkInDate,
        DateTime? checkOutDate,
        int? guests,
        RoomType? roomType);

    Task<IReadOnlyList<Room>> GetFeaturedRoomsAsync(int count = 4);
}
