
namespace TheBabyCowBoy.ContentApi.Models;

public class JournalEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string? Mood { get; set; }

    public string Category { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new();

    // New entries are private by default.
    public bool IsPublic { get; set; } = false;

    public DateTimeOffset CreatedAt { get; set; } =
        DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } =
        DateTimeOffset.UtcNow;

    public List<JournalImage> Images { get; set; } = new();
}
