
namespace TheBabyCowBoy.ContentApi.DTOs.Travel;

public class TravelTripRequestDto
{
    public string Title { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? CoverImageUrl { get; set; }
}

public class TravelPhotoRequestDto
{
    public string ImageUrl { get; set; } = string.Empty;

    public string? Caption { get; set; }
}

public class TravelPhotoResponseDto
{
    public Guid Id { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string? Caption { get; set; }
}

public class TravelTripResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? CoverImageUrl { get; set; }

    public List<TravelPhotoResponseDto> Photos { get; set; } = new();
}
