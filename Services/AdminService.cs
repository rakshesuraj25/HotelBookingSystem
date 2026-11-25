using HotelBookingSystem.Areas.Admin.ViewModels;
using HotelBookingSystem.Data;
using HotelBookingSystem.Interfaces;
using HotelBookingSystem.Models;
using HotelBookingSystem.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Services;

/// <summary>
/// Provides administrative queries and management operations for rooms, bookings, and guest accounts.
/// </summary>
public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _context;

    public AdminService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdminDashboardViewModel> GetDashboardAsync()
    {
        var totalRooms = await _context.Rooms.CountAsync();
        var availableRooms = await _context.Rooms.CountAsync(r => r.RoomStatus == RoomStatus.Available);
        var totalBookings = await _context.Bookings.CountAsync();
        var pendingBookings = await _context.Bookings.CountAsync(b => b.BookingStatus == BookingStatus.Pending);
        var confirmedBookings = await _context.Bookings.CountAsync(b => b.BookingStatus == BookingStatus.Confirmed);
        var cancelledBookings = await _context.Bookings.CountAsync(b => b.BookingStatus == BookingStatus.Cancelled);
        var completedBookings = await _context.Bookings.CountAsync(b => b.BookingStatus == BookingStatus.Completed);
        var registeredGuests = await _context.ApplicationUsers.CountAsync(u => u.Role == UserRole.Guest);

        var recentBookings = await _context.Bookings
            .AsNoTracking()
            .OrderByDescending(b => b.CreatedAt)
            .Take(8)
            .Select(b => new AdminBookingRowViewModel
            {
                Id = b.Id,
                BookingReference = b.BookingReference,
                GuestName = b.User.FullName,
                GuestEmail = b.User.Email,
                RoomName = b.Room.Name,
                RoomNumber = b.Room.RoomNumber,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                TotalAmount = b.TotalAmount,
                BookingStatus = b.BookingStatus,
                CreatedAt = b.CreatedAt
            })
            .ToListAsync();

        return new AdminDashboardViewModel
        {
            TotalRooms = totalRooms,
            AvailableRooms = availableRooms,
            TotalBookings = totalBookings,
            PendingBookings = pendingBookings,
            ConfirmedBookings = confirmedBookings,
            CancelledBookings = cancelledBookings,
            CompletedBookings = completedBookings,
            RegisteredGuests = registeredGuests,
            RecentBookings = recentBookings
        };
    }

    public async Task<IReadOnlyList<Room>> GetAllRoomsAsync()
    {
        return await _context.Rooms
            .AsNoTracking()
            .OrderBy(r => r.RoomNumber)
            .ToListAsync();
    }

    public async Task<Room?> GetRoomByIdAsync(int id)
    {
        return await _context.Rooms
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<AdminOperationResult> CreateRoomAsync(AdminRoomFormViewModel model)
    {
        var roomNumber = model.RoomNumber.Trim();
        var exists = await _context.Rooms.AnyAsync(r => r.RoomNumber.ToLower() == roomNumber.ToLower());
        if (exists)
        {
            return new AdminOperationResult(false, $"Room number '{roomNumber}' already exists.");
        }

        var room = new Room
        {
            RoomNumber = roomNumber,
            Name = model.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
            RoomType = model.RoomType,
            PricePerNight = model.PricePerNight,
            Capacity = model.Capacity,
            BedType = model.BedType.Trim(),
            ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl) ? null : model.ImageUrl.Trim(),
            Amenities = string.IsNullOrWhiteSpace(model.Amenities) ? null : model.Amenities.Trim(),
            RoomStatus = model.RoomStatus,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();
        return new AdminOperationResult(true);
    }

    public async Task<AdminOperationResult> UpdateRoomAsync(AdminRoomFormViewModel model)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == model.Id);
        if (room is null)
        {
            return new AdminOperationResult(false, "Room was not found.");
        }

        var roomNumber = model.RoomNumber.Trim();
        var duplicateNumber = await _context.Rooms
            .AnyAsync(r => r.Id != model.Id && r.RoomNumber.ToLower() == roomNumber.ToLower());

        if (duplicateNumber)
        {
            return new AdminOperationResult(false, $"Another room already uses room number '{roomNumber}'.");
        }

        room.RoomNumber = roomNumber;
        room.Name = model.Name.Trim();
        room.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
        room.RoomType = model.RoomType;
        room.PricePerNight = model.PricePerNight;
        room.Capacity = model.Capacity;
        room.BedType = model.BedType.Trim();
        room.ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl) ? null : model.ImageUrl.Trim();
        room.Amenities = string.IsNullOrWhiteSpace(model.Amenities) ? null : model.Amenities.Trim();
        room.RoomStatus = model.RoomStatus;
        room.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return new AdminOperationResult(true);
    }

    public async Task<AdminOperationResult> ChangeRoomStatusAsync(int roomId, RoomStatus status)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
        if (room is null)
        {
            return new AdminOperationResult(false, "Room was not found.");
        }

        room.RoomStatus = status;
        room.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return new AdminOperationResult(true);
    }

    public async Task<AdminOperationResult> DeleteRoomAsync(int roomId)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
        if (room is null)
        {
            return new AdminOperationResult(false, "Room was not found.");
        }

        var hasBookings = await _context.Bookings.AnyAsync(b => b.RoomId == roomId);
        if (hasBookings)
        {
            return new AdminOperationResult(
                false,
                "This room has existing booking history and cannot be deleted. Set its status to Inactive instead.");
        }

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
        return new AdminOperationResult(true);
    }

    public async Task<IReadOnlyList<AdminBookingRowViewModel>> GetBookingsAsync(string? searchReference, BookingStatus? status)
    {
        var query = _context.Bookings.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchReference))
        {
            var term = searchReference.Trim().ToLower();
            query = query.Where(b => b.BookingReference.ToLower().Contains(term));
        }

        if (status.HasValue)
        {
            query = query.Where(b => b.BookingStatus == status.Value);
        }

        return await query
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => new AdminBookingRowViewModel
            {
                Id = b.Id,
                BookingReference = b.BookingReference,
                GuestName = b.User.FullName,
                GuestEmail = b.User.Email,
                RoomName = b.Room.Name,
                RoomNumber = b.Room.RoomNumber,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                TotalAmount = b.TotalAmount,
                BookingStatus = b.BookingStatus,
                CreatedAt = b.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<AdminBookingDetailsViewModel?> GetBookingDetailsAsync(int bookingId)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Where(b => b.Id == bookingId)
            .Select(b => new AdminBookingDetailsViewModel
            {
                Id = b.Id,
                BookingReference = b.BookingReference,
                BookingStatus = b.BookingStatus,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                NumberOfNights = b.NumberOfNights,
                NumberOfGuests = b.NumberOfGuests,
                PricePerNight = b.PricePerNight,
                TotalAmount = b.TotalAmount,
                SpecialRequests = b.SpecialRequests,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt,
                GuestId = b.User.Id,
                GuestName = b.User.FullName,
                GuestEmail = b.User.Email,
                GuestPhone = b.User.PhoneNumber,
                RoomId = b.Room.Id,
                RoomNumber = b.Room.RoomNumber,
                RoomName = b.Room.Name,
                RoomType = b.Room.RoomType
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<AdminGuestViewModel>> GetGuestsAsync()
    {
        return await _context.ApplicationUsers
            .AsNoTracking()
            .Where(u => u.Role == UserRole.Guest)
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new AdminGuestViewModel
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                BookingCount = u.Bookings.Count
            })
            .ToListAsync();
    }

    public async Task<AdminGuestDetailsViewModel?> GetGuestDetailsAsync(int guestId)
    {
        return await _context.ApplicationUsers
            .AsNoTracking()
            .Where(u => u.Id == guestId && u.Role == UserRole.Guest)
            .Select(u => new AdminGuestDetailsViewModel
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                Bookings = u.Bookings
                    .OrderByDescending(b => b.CreatedAt)
                    .Select(b => new AdminBookingRowViewModel
                    {
                        Id = b.Id,
                        BookingReference = b.BookingReference,
                        GuestName = u.FullName,
                        GuestEmail = u.Email,
                        RoomName = b.Room.Name,
                        RoomNumber = b.Room.RoomNumber,
                        CheckInDate = b.CheckInDate,
                        CheckOutDate = b.CheckOutDate,
                        TotalAmount = b.TotalAmount,
                        BookingStatus = b.BookingStatus,
                        CreatedAt = b.CreatedAt
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<AdminOperationResult> ToggleGuestActiveAsync(int guestId)
    {
        var guest = await _context.ApplicationUsers
            .FirstOrDefaultAsync(u => u.Id == guestId && u.Role == UserRole.Guest);

        if (guest is null)
        {
            return new AdminOperationResult(false, "Guest account was not found.");
        }

        guest.IsActive = !guest.IsActive;
        await _context.SaveChangesAsync();
        return new AdminOperationResult(true);
    }
}
