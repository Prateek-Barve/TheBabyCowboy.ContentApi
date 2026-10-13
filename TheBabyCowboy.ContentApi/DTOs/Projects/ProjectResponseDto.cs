
namespace TheBabyCowBoy.ContentApi.DTOs.Projects;

public class ProjectResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string ShortDescription { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? ThumbnailUrl { get; set; }

    public string? GitHubUrl { get; set; }

    public List<ProjectSkillDto> Skills { get; set; } = new();
}

public class ProjectSkillDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? IconUrl { get; set; }

    public string CategoryName { get; set; } = string.Empty;
}
