
namespace TheBabyCowBoy.ContentApi.Models;

public class JournalImage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid JournalEntryId { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string? Caption { get; set; }

    public int SortOrder { get; set; }

    public JournalEntry JournalEntry { get; set; } = null!;
}
