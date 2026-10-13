
using TheBabyCowBoy.ContentApi.DTOs.Projects;

namespace TheBabyCowBoy.ContentApi.Services.Interfaces;

public interface IProjectService
{
    Task<List<ProjectResponseDto>> GetAllAsync();

    Task<ProjectResponseDto?> GetByIdAsync(Guid id);

    Task<ProjectResponseDto> CreateAsync(ProjectRequestDto request);

    Task<ProjectResponseDto?> UpdateAsync(
        Guid id,
        ProjectRequestDto request);

    Task<bool> DeleteAsync(Guid id);
}
