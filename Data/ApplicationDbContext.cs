using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Data;

/// <summary>
/// Entity Framework Core database context for the Hotel Room Booking System.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Domain entity DbSets (e.g., Rooms, Bookings, Guests) will be added in Phase 2.
}
