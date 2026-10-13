
namespace TheBabyCowBoy.ContentApi.DTOs.Skills;

public class SkillRequestDto
{
    public string Name { get; set; } = string.Empty;

    public string? IconUrl { get; set; }

    public Guid SkillCategoryId { get; set; }
}

public class SkillResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? IconUrl { get; set; }

    public Guid SkillCategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;
}
