
using TheBabyCowBoy.ContentApi.DTOs.Skills;

namespace TheBabyCowBoy.ContentApi.Services.Interfaces;

public interface ISkillService
{
    Task<List<SkillResponseDto>> GetAllAsync(Guid? categoryId = null);
    Task<SkillResponseDto?> GetByIdAsync(Guid id);
    Task<SkillResponseDto> CreateAsync(SkillRequestDto request);
    Task<SkillResponseDto?> UpdateAsync(Guid id, SkillRequestDto request);
    Task<bool> DeleteAsync(Guid id);
}
