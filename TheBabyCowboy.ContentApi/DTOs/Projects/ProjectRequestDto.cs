
namespace TheBabyCowBoy.ContentApi.DTOs.Projects;

public class ProjectRequestDto
{
    public string Title { get; set; } = string.Empty;

    public string ShortDescription { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = "InProgress";

    public string? ThumbnailUrl { get; set; }

    public string? GitHubUrl { get; set; }

    public List<Guid> SkillIds { get; set; } = new();
}
