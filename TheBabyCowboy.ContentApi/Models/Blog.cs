namespace TheBabyCowBoy.ContentApi.Models;

public class Blog
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;

    public string Excerpt { get; set; } = string.Empty;

    // Store the complete article as Markdown.
    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    // Stored as PostgreSQL JSONB.
    public List<string> Tags { get; set; } = new();

    public string? CoverImageUrl { get; set; }
    public string ExternalUrl { get; set; } = string.Empty;

    public DateTimeOffset? PublishedAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; } =
        DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } =
        DateTimeOffset.UtcNow;
}