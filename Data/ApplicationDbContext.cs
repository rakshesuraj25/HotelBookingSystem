using HotelBookingSystem.Models;
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

    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();

    public DbSet<Room> Rooms => Set<Room>();

    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();

            entity.Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(20);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasIndex(r => r.RoomNumber).IsUnique();
            entity.HasIndex(r => new { r.RoomType, r.RoomStatus });

            entity.Property(r => r.PricePerNight).HasPrecision(10, 2);

            entity.Property(r => r.RoomType)
                .HasConversion<string>()
                .HasMaxLength(30);

            entity.Property(r => r.RoomStatus)
                .HasConversion<string>()
                .HasMaxLength(30);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasIndex(b => b.BookingReference).IsUnique();
            entity.HasIndex(b => new { b.RoomId, b.CheckInDate, b.CheckOutDate });
            entity.HasIndex(b => b.UserId);
            entity.HasIndex(b => b.BookingStatus);

            entity.Property(b => b.PricePerNight).HasPrecision(10, 2);
            entity.Property(b => b.TotalAmount).HasPrecision(10, 2);

            entity.Property(b => b.BookingStatus)
                .HasConversion<string>()
                .HasMaxLength(30);

            // Prevent deleting users or rooms that have existing booking history.
            entity.HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
