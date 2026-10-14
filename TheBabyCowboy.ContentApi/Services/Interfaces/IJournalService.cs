
using TheBabyCowBoy.ContentApi.DTOs.Journals;

namespace TheBabyCowBoy.ContentApi.Services.Interfaces;

public interface IJournalService
{
    Task<List<JournalResponseDto>> GetAllAsync(
        bool includePrivate,
        string? search,
        string? mood,
        string? category,
        string? tag,
        DateTimeOffset? from,
        DateTimeOffset? to);

    Task<JournalResponseDto?> GetByIdAsync(
        Guid id,
        bool includePrivate);

    Task<JournalResponseDto> CreateAsync(
        JournalRequestDto request);

    Task<JournalResponseDto?> UpdateAsync(
        Guid id,
        JournalRequestDto request);

    Task<bool> DeleteAsync(Guid id);
}
