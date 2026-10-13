
namespace TheBabyCowboy.ContentApi.Models;

public class Skill
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string? IconUrl { get; set; }

    public Guid SkillCategoryId { get; set; }

    public SkillCategory? SkillCategory { get; set; }

    public ICollection<ProjectSkill> ProjectSkills { get; set; }
        = new List<ProjectSkill>();
}
