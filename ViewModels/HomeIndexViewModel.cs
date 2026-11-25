namespace HotelBookingSystem.ViewModels;

public class HomeIndexViewModel
{
    public RoomSearchViewModel Search { get; set; } = new();
    public IReadOnlyList<RoomCardViewModel> FeaturedRooms { get; set; } = Array.Empty<RoomCardViewModel>();
}
