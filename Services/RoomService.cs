using HotelBookingSystem.Data;
using HotelBookingSystem.Interfaces;
using HotelBookingSystem.Models;
using HotelBookingSystem.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Services;

/// <summary>
/// Implements room querying and date-range availability filtering.
/// </summary>
public class RoomService : IRoomService
{
    private readonly ApplicationDbContext _context;

    public RoomService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Room>> GetAllRoomsAsync()
    {
        return await _context.Rooms
            .AsNoTracking()
            .Where(r => r.RoomStatus == RoomStatus.Available)
            .OrderBy(r => r.RoomNumber)
            .ToListAsync();
    }

    public async Task<Room?> GetRoomByIdAsync(int id)
    {
        return await _context.Rooms
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id && r.RoomStatus == RoomStatus.Available);
    }

    public async Task<IReadOnlyList<Room>> SearchAvailableRoomsAsync(
        DateTime? checkInDate,
        DateTime? checkOutDate,
        int? guests,
        RoomType? roomType)
    {
        var query = _context.Rooms
            .AsNoTracking()
            .Where(r => r.RoomStatus == RoomStatus.Available);

        if (guests.HasValue && guests.Value > 0)
        {
            query = query.Where(r => r.Capacity >= guests.Value);
        }

        if (roomType.HasValue)
        {
            query = query.Where(r => r.RoomType == roomType.Value);
        }

        if (checkInDate.HasValue && checkOutDate.HasValue)
        {
            var checkIn = checkInDate.Value.Date;
            var checkOut = checkOutDate.Value.Date;

            if (checkOut <= checkIn)
            {
                throw new ArgumentException("Check-out date must be later than check-in date.");
            }

            query = query.Where(r => !r.Bookings.Any(b =>
                (b.BookingStatus == BookingStatus.Pending || b.BookingStatus == BookingStatus.Confirmed) &&
                checkIn < b.CheckOutDate &&
                checkOut > b.CheckInDate));
        }

        return await query
            .OrderBy(r => r.PricePerNight)
            .ThenBy(r => r.RoomNumber)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Room>> GetFeaturedRoomsAsync(int count = 4)
    {
        var takeCount = count > 0 ? count : 4;

        return await _context.Rooms
            .AsNoTracking()
            .Where(r => r.RoomStatus == RoomStatus.Available)
            .OrderBy(r => r.RoomType)
            .ThenBy(r => r.PricePerNight)
            .Take(takeCount)
            .ToListAsync();
    }
}
