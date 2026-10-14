
namespace TheBabyCowBoy.ContentApi.DTOs.Blogs;

public class BlogRequestDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new();

    public string? CoverImageUrl { get; set; }

    public string ExternalUrl { get; set; } = string.Empty;

    public DateTimeOffset? PublishedAt { get; set; }
}

public class BlogResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new();

    public string? CoverImageUrl { get; set; }

    public string ExternalUrl { get; set; } = string.Empty;

    public DateTimeOffset? PublishedAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
