using HotelBookingSystem.Models;
using HotelBookingSystem.Models.Enums;

namespace HotelBookingSystem.Interfaces;

/// <summary>
/// Represents the outcome of a booking operation.
/// </summary>
public sealed record BookingResult(bool Succeeded, string? ErrorMessage = null, Booking? Booking = null);

/// <summary>
/// Provides server-validated booking lifecycle operations.
/// </summary>
public interface IBookingService
{
    Task<BookingResult> CreateBookingAsync(
        int userId,
        int roomId,
        DateTime checkInDate,
        DateTime checkOutDate,
        int numberOfGuests,
        string? specialRequests = null);

    Task<Booking?> GetBookingByIdAsync(int bookingId);

    Task<IReadOnlyList<Booking>> GetUserBookingsAsync(int userId);

    Task<BookingResult> CancelBookingAsync(int bookingId, int userId, bool isAdmin = false);

    Task<BookingResult> UpdateBookingStatusAsync(int bookingId, BookingStatus newStatus);
}
