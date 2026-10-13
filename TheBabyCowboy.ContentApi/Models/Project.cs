
namespace TheBabyCowboy.ContentApi.Models;

public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;

    public string ShortDescription { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? ThumbnailUrl { get; set; }

    public string? GitHubUrl { get; set; }

    public ICollection<ProjectSkill> ProjectSkills { get; set; }
        = new List<ProjectSkill>();
}
