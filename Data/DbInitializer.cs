using HotelBookingSystem.Interfaces;
using HotelBookingSystem.Models;
using HotelBookingSystem.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Data;

/// <summary>
/// Seeds initial development data (hotel rooms and an optional configured admin account) idempotently.
/// </summary>
public class DbInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(
        ApplicationDbContext context,
        IPasswordHasherService passwordHasher,
        IConfiguration configuration,
        ILogger<DbInitializer> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        await SeedRoomsAsync();
        await SeedAdminUserAsync();
    }

    private async Task SeedRoomsAsync()
    {
        var seedRooms = GetSeedRooms();
        var existingRooms = await _context.Rooms
            .Where(r => seedRooms.Select(sr => sr.RoomNumber).Contains(r.RoomNumber))
            .ToDictionaryAsync(r => r.RoomNumber, StringComparer.OrdinalIgnoreCase);

        var newRooms = new List<Room>();
        var updatedCount = 0;

        foreach (var seed in seedRooms)
        {
            if (!existingRooms.TryGetValue(seed.RoomNumber, out var existing))
            {
                newRooms.Add(seed);
                continue;
            }

            // Safely update ImageUrl or legacy Phase 4 USD prices without touching existing bookings.
            var needsImageUpdate = !string.Equals(existing.ImageUrl, seed.ImageUrl, StringComparison.OrdinalIgnoreCase);

            var needsInrPriceUpdate = existing.PricePerNight < 1000m;

            if (needsImageUpdate || needsInrPriceUpdate)
            {
                if (needsInrPriceUpdate)
                {
                    existing.PricePerNight = seed.PricePerNight;
                }

                if (needsImageUpdate)
                {
                    existing.ImageUrl = seed.ImageUrl;
                }

                existing.UpdatedAt = DateTime.UtcNow;
                updatedCount++;
            }
        }

        if (newRooms.Count > 0)
        {
            await _context.Rooms.AddRangeAsync(newRooms);
        }

        if (newRooms.Count > 0 || updatedCount > 0)
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Room catalog initialized: {Added} added, {Updated} updated.", newRooms.Count, updatedCount);
        }
    }

    private async Task SeedAdminUserAsync()
    {
        var email = _configuration["SeedAdmin:Email"]?.Trim();
        var fullName = _configuration["SeedAdmin:FullName"]?.Trim();
        var password = _configuration["SeedAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("SeedAdmin credentials are not configured in User Secrets or environment variables; skipping initial admin seeding.");
            return;
        }

        var normalizedEmail = email.ToLowerInvariant();
        var exists = await _context.ApplicationUsers
            .AnyAsync(u => u.Email.ToLower() == normalizedEmail);

        if (exists)
        {
            return;
        }

        var admin = new ApplicationUser
        {
            FullName = string.IsNullOrWhiteSpace(fullName) ? "Aarohan Residency Administrator" : fullName,
            Email = normalizedEmail,
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        admin.PasswordHash = _passwordHasher.HashPassword(admin, password);

        await _context.ApplicationUsers.AddAsync(admin);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded initial admin account for {Email}.", normalizedEmail);
    }

    private static List<Room> GetSeedRooms()
    {
        var now = DateTime.UtcNow;

        return
        [
            new Room
            {
                RoomNumber = "101",
                Name = "Courtyard Standard Room",
                Description = "Quiet ground-floor room overlooking the landscaped courtyard with comfortable essentials for solo travelers or couples.",
                RoomType = RoomType.Standard,
                PricePerNight = 2000.00m,
                Capacity = 2,
                BedType = "1 Queen Bed",
                ImageUrl = "/images/rooms/room-101.jpg",
                Amenities = "Free Wi-Fi, Air Conditioning, Smart TV, Work Desk, Tea & Coffee Maker",
                RoomStatus = RoomStatus.Available,
                CreatedAt = now
            },
            new Room
            {
                RoomNumber = "102",
                Name = "City View Standard Twin",
                Description = "Bright standard room featuring twin beds, blackout curtains, and an ergonomic workspace for business or leisure stays.",
                RoomType = RoomType.Standard,
                PricePerNight = 2100.00m,
                Capacity = 2,
                BedType = "2 Twin Beds",
                ImageUrl = "/images/rooms/room-102.jpg",
                Amenities = "Free Wi-Fi, Air Conditioning, Smart TV, Work Desk, In-Room Safe",
                RoomStatus = RoomStatus.Available,
                CreatedAt = now
            },
            new Room
            {
                RoomNumber = "103",
                Name = "Classic Comfort Standard",
                Description = "Warmly appointed room with crisp bedding, rainfall shower, and reliable high-speed wireless connectivity.",
                RoomType = RoomType.Standard,
                PricePerNight = 2200.00m,
                Capacity = 2,
                BedType = "1 Queen Bed",
                ImageUrl = "/images/rooms/room-103.jpg",
                Amenities = "Free Wi-Fi, Air Conditioning, Smart TV, Rainfall Shower, Mini Fridge",
                RoomStatus = RoomStatus.Available,
                CreatedAt = now
            },
            new Room
            {
                RoomNumber = "201",
                Name = "Garden Balcony Deluxe",
                Description = "Spacious deluxe room with a private balcony overlooking the gardens, lounge seating, and upgraded bath amenities.",
                RoomType = RoomType.Deluxe,
                PricePerNight = 3500.00m,
                Capacity = 3,
                BedType = "1 King Bed + Sofa",
                ImageUrl = "/images/rooms/room-201.jpg",
                Amenities = "Free Wi-Fi, Private Balcony, Air Conditioning, Smart TV, Mini Fridge, Coffee Maker",
                RoomStatus = RoomStatus.Available,
                CreatedAt = now
            },
            new Room
            {
                RoomNumber = "202",
                Name = "Skyline Corner Deluxe",
                Description = "Corner deluxe room bathed in natural light through wide windows with pleasant city views and a lounge chair.",
                RoomType = RoomType.Deluxe,
                PricePerNight = 3700.00m,
                Capacity = 2,
                BedType = "1 King Bed",
                ImageUrl = "/images/rooms/room-202.jpg",
                Amenities = "Free Wi-Fi, Wide Windows, Air Conditioning, Smart TV, Work Desk, Coffee Maker",
                RoomStatus = RoomStatus.Available,
                CreatedAt = now
            },
            new Room
            {
                RoomNumber = "203",
                Name = "Terrace Wing Deluxe",
                Description = "Well-appointed deluxe accommodation with terrace access, restful lighting, and dedicated seating area.",
                RoomType = RoomType.Deluxe,
                PricePerNight = 3600.00m,
                Capacity = 3,
                BedType = "1 King Bed + Daybed",
                ImageUrl = "/images/rooms/room-203.jpg",
                Amenities = "Free Wi-Fi, Terrace Access, Air Conditioning, Smart TV, Mini Fridge, Rain Shower",
                RoomStatus = RoomStatus.Available,
                CreatedAt = now
            },
            new Room
            {
                RoomNumber = "301",
                Name = "Executive Junior Suite",
                Description = "Open-plan suite combining a king bedroom with a separate lounge and dining nook for extended stays.",
                RoomType = RoomType.Suite,
                PricePerNight = 5500.00m,
                Capacity = 3,
                BedType = "1 King Bed + Lounge Sofa",
                ImageUrl = "/images/rooms/room-301.jpg",
                Amenities = "Free Wi-Fi, Separate Lounge, Smart TV, Coffee Maker, Complimentary Breakfast",
                RoomStatus = RoomStatus.Available,
                CreatedAt = now
            },
            new Room
            {
                RoomNumber = "302",
                Name = "Ambassador Corner Suite",
                Description = "Refined suite featuring a private living room, spacious bathroom, and executive desk for work and relaxation.",
                RoomType = RoomType.Suite,
                PricePerNight = 6000.00m,
                Capacity = 4,
                BedType = "1 King Bed + Sofa Bed",
                ImageUrl = "/images/rooms/room-302.jpg",
                Amenities = "Free Wi-Fi, Living Room, Smart TV, Mini Fridge, Lounge Access, Breakfast Included",
                RoomStatus = RoomStatus.Available,
                CreatedAt = now
            },
            new Room
            {
                RoomNumber = "303",
                Name = "Aarohan Signature Suite",
                Description = "Top-floor signature suite with a private dining area, generous terrace, and panoramic city views.",
                RoomType = RoomType.Suite,
                PricePerNight = 6800.00m,
                Capacity = 4,
                BedType = "1 King Bed + 1 Queen Sofa Bed",
                ImageUrl = "/images/rooms/room-303.jpg",
                Amenities = "Free Wi-Fi, Private Terrace, Dining Area, Concierge Service, Complimentary Breakfast",
                RoomStatus = RoomStatus.Available,
                CreatedAt = now
            },
            new Room
            {
                RoomNumber = "401",
                Name = "Sunshine Family Studio",
                Description = "Generously sized family room with two queen beds, family-friendly layout, and ample luggage storage.",
                RoomType = RoomType.Family,
                PricePerNight = 4500.00m,
                Capacity = 4,
                BedType = "2 Queen Beds",
                ImageUrl = "/images/rooms/room-401.jpg",
                Amenities = "Free Wi-Fi, Air Conditioning, Smart TV, Mini Fridge, Extra Storage",
                RoomStatus = RoomStatus.Available,
                CreatedAt = now
            },
            new Room
            {
                RoomNumber = "402",
                Name = "Grand Two-Bedroom Family",
                Description = "Interconnected two-bedroom family accommodation designed for larger families seeking privacy and comfort.",
                RoomType = RoomType.Family,
                PricePerNight = 5200.00m,
                Capacity = 6,
                BedType = "1 King Bed + 2 Twin Beds + Sofa Bed",
                ImageUrl = "/images/rooms/room-402.jpg",
                Amenities = "Free Wi-Fi, Two Bathrooms, Kitchenette, Smart TVs, Air Conditioning",
                RoomStatus = RoomStatus.Available,
                CreatedAt = now
            },
            new Room
            {
                RoomNumber = "403",
                Name = "Garden Wing Family Loft",
                Description = "Spacious family room with a quiet sleeping alcove, dining table, and convenient access to garden areas.",
                RoomType = RoomType.Family,
                PricePerNight = 4800.00m,
                Capacity = 5,
                BedType = "1 Queen Bed + 2 Twin Beds + Daybed",
                ImageUrl = "/images/rooms/family-room.jpg",
                Amenities = "Free Wi-Fi, Dining Table, Smart TV, Mini Fridge, Air Conditioning",
                RoomStatus = RoomStatus.Available,
                CreatedAt = now
            }
        ];
    }
}
