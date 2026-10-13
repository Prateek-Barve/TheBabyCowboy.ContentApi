
namespace TheBabyCowboy.ContentApi.Models;

public class SkillCategory
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public ICollection<Skill> Skills { get; set; }
        = new List<Skill>();
}
