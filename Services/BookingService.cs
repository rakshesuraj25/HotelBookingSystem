using HotelBookingSystem.Data;
using HotelBookingSystem.Interfaces;
using HotelBookingSystem.Models;
using HotelBookingSystem.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Services;

/// <summary>
/// Implements booking creation, retrieval, cancellation, and status transitions with server-side validation.
/// </summary>
public class BookingService : IBookingService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BookingService> _logger;

    public BookingService(ApplicationDbContext context, ILogger<BookingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BookingResult> CreateBookingAsync(
        int userId,
        int roomId,
        DateTime checkInDate,
        DateTime checkOutDate,
        int numberOfGuests,
        string? specialRequests = null)
    {
        var checkIn = checkInDate.Date;
        var checkOut = checkOutDate.Date;

        if (checkIn < DateTime.UtcNow.Date)
        {
            return new BookingResult(false, "Check-in date cannot be in the past.");
        }

        if (checkOut <= checkIn)
        {
            return new BookingResult(false, "Check-out date must be later than check-in date.");
        }

        if (numberOfGuests <= 0)
        {
            return new BookingResult(false, "Number of guests must be at least 1.");
        }

        var user = await _context.ApplicationUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null || !user.IsActive)
        {
            return new BookingResult(false, "User account is not valid or is inactive.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Acquire an InnoDB row-level exclusive lock on the target Room before checking availability.
            // ToListAsync avoids wrapping FOR UPDATE in a derived subquery, which MySQL 8.0 rejects.
            var lockedRooms = await _context.Rooms
                .FromSqlInterpolated($"SELECT * FROM Rooms WHERE Id = {roomId} FOR UPDATE")
                .AsNoTracking()
                .ToListAsync();

            var room = lockedRooms.FirstOrDefault();

            if (room is null)
            {
                await transaction.RollbackAsync();
                return new BookingResult(false, "Selected room was not found.");
            }

            if (room.RoomStatus != RoomStatus.Available)
            {
                await transaction.RollbackAsync();
                return new BookingResult(false, "Selected room is currently unavailable for booking.");
            }

            if (numberOfGuests > room.Capacity)
            {
                await transaction.RollbackAsync();
                return new BookingResult(false, $"Number of guests ({numberOfGuests}) exceeds room capacity ({room.Capacity}).");
            }

            // Only Pending and Confirmed bookings block overlapping dates; Cancelled and Completed do not.
            var hasOverlap = await _context.Bookings.AnyAsync(b =>
                b.RoomId == roomId &&
                (b.BookingStatus == BookingStatus.Pending || b.BookingStatus == BookingStatus.Confirmed) &&
                checkIn < b.CheckOutDate &&
                checkOut > b.CheckInDate);

            if (hasOverlap)
            {
                await transaction.RollbackAsync();
                return new BookingResult(false, "The selected room is already reserved for those dates.");
            }

            var numberOfNights = (checkOut - checkIn).Days;
            var totalAmount = room.PricePerNight * numberOfNights;
            var bookingReference = await GenerateUniqueReferenceAsync();

            var booking = new Booking
            {
                BookingReference = bookingReference,
                UserId = userId,
                RoomId = roomId,
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                NumberOfGuests = numberOfGuests,
                NumberOfNights = numberOfNights,
                PricePerNight = room.PricePerNight,
                TotalAmount = totalAmount,
                BookingStatus = BookingStatus.Pending,
                SpecialRequests = string.IsNullOrWhiteSpace(specialRequests) ? null : specialRequests.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new BookingResult(true, null, booking);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating booking for RoomId {RoomId} and UserId {UserId}.", roomId, userId);
            throw;
        }
    }

    public async Task<Booking?> GetBookingByIdAsync(int bookingId)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Include(b => b.Room)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == bookingId);
    }

    public async Task<IReadOnlyList<Booking>> GetUserBookingsAsync(int userId)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Include(b => b.Room)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<BookingResult> CancelBookingAsync(int bookingId, int userId, bool isAdmin = false)
    {
        var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
        if (booking is null)
        {
            return new BookingResult(false, "Booking was not found.");
        }

        if (!isAdmin && booking.UserId != userId)
        {
            return new BookingResult(false, "You are not authorized to cancel this booking.");
        }

        if (booking.BookingStatus is BookingStatus.Cancelled or BookingStatus.Completed)
        {
            return new BookingResult(false, $"Booking is already {booking.BookingStatus}.");
        }

        booking.BookingStatus = BookingStatus.Cancelled;
        booking.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return new BookingResult(true, null, booking);
    }

    public async Task<BookingResult> UpdateBookingStatusAsync(int bookingId, BookingStatus newStatus)
    {
        var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
        if (booking is null)
        {
            return new BookingResult(false, "Booking was not found.");
        }

        if (booking.BookingStatus == BookingStatus.Cancelled && newStatus != BookingStatus.Cancelled)
        {
            return new BookingResult(false, "Cannot change the status of a cancelled booking.");
        }

        booking.BookingStatus = newStatus;
        booking.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return new BookingResult(true, null, booking);
    }

    private async Task<string> GenerateUniqueReferenceAsync()
    {
        string reference;
        do
        {
            var token = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
            reference = $"HBS-{DateTime.UtcNow:yyyyMMdd}-{token}";
        }
        while (await _context.Bookings.AnyAsync(b => b.BookingReference == reference));

        return reference;
    }
}
