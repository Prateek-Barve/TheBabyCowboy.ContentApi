
namespace TheBabyCowBoy.ContentApi.DTOs.Journals;

public class JournalImageRequestDto
{
    public string ImageUrl { get; set; } = string.Empty;

    public string? Caption { get; set; }

    public int SortOrder { get; set; }
}

public class JournalRequestDto
{
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string? Mood { get; set; }

    public string Category { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new();

    public bool IsPublic { get; set; } = false;

    public List<JournalImageRequestDto> Images { get; set; } = new();
}

public class JournalImageResponseDto
{
    public Guid Id { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string? Caption { get; set; }

    public int SortOrder { get; set; }
}

public class JournalResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string? Mood { get; set; }

    public string Category { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new();

    public bool IsPublic { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public List<JournalImageResponseDto> Images { get; set; } = new();
}
