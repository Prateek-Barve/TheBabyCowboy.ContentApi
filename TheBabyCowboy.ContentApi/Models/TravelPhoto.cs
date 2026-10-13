
namespace TheBabyCowBoy.ContentApi.Models;

public class TravelPhoto
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TravelTripId { get; set; }

    public TravelTrip TravelTrip { get; set; } = null!;

    public string ImageUrl { get; set; } = string.Empty;

    public string? Caption { get; set; }
}
