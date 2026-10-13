
using TheBabyCowBoy.ContentApi.DTOs.SkillCategories;

namespace TheBabyCowBoy.ContentApi.Services.Interfaces;

public interface ISkillCategoryService
{
    Task<List<SkillCategoryResponseDto>> GetAllAsync();
    Task<SkillCategoryResponseDto?> GetByIdAsync(Guid id);
    Task<SkillCategoryResponseDto> CreateAsync(SkillCategoryRequestDto request);
    Task<SkillCategoryResponseDto?> UpdateAsync(Guid id, SkillCategoryRequestDto request);
    Task<bool> DeleteAsync(Guid id);
}
