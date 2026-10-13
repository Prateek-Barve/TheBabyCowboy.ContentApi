
namespace TheBabyCowBoy.ContentApi.DTOs.SkillCategories;

public class SkillCategoryRequestDto
{
    public string Name { get; set; } = string.Empty;
}

public class SkillCategoryResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
