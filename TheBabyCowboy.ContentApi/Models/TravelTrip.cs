
namespace TheBabyCowBoy.ContentApi.Models;

public class TravelTrip
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }

    public ICollection<TravelPhoto> Photos { get; set; }
        = new List<TravelPhoto>();
}
