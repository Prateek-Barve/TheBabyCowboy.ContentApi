
using TheBabyCowBoy.ContentApi.DTOs.Blogs;

namespace TheBabyCowBoy.ContentApi.Services.Interfaces;

public interface IBlogService
{
    Task<List<BlogResponseDto>> GetAllAsync();

    Task<BlogResponseDto?> GetByIdAsync(Guid id);

    Task<BlogResponseDto> CreateAsync(BlogRequestDto request);

    Task<BlogResponseDto?> UpdateAsync(
        Guid id,
        BlogRequestDto request);

    Task<bool> DeleteAsync(Guid id);
}
