using HotelBookingSystem.Areas.Admin.ViewModels;
using HotelBookingSystem.Models;
using HotelBookingSystem.Models.Enums;

namespace HotelBookingSystem.Interfaces;

public sealed record AdminOperationResult(bool Succeeded, string? ErrorMessage = null);

public interface IAdminService
{
    Task<AdminDashboardViewModel> GetDashboardAsync();

    Task<IReadOnlyList<Room>> GetAllRoomsAsync();
    Task<Room?> GetRoomByIdAsync(int id);
    Task<AdminOperationResult> CreateRoomAsync(AdminRoomFormViewModel model);
    Task<AdminOperationResult> UpdateRoomAsync(AdminRoomFormViewModel model);
    Task<AdminOperationResult> ChangeRoomStatusAsync(int roomId, RoomStatus status);
    Task<AdminOperationResult> DeleteRoomAsync(int roomId);

    Task<IReadOnlyList<AdminBookingRowViewModel>> GetBookingsAsync(string? searchReference, BookingStatus? status);
    Task<AdminBookingDetailsViewModel?> GetBookingDetailsAsync(int bookingId);

    Task<IReadOnlyList<AdminGuestViewModel>> GetGuestsAsync();
    Task<AdminGuestDetailsViewModel?> GetGuestDetailsAsync(int guestId);
    Task<AdminOperationResult> ToggleGuestActiveAsync(int guestId);
}
