
using TheBabyCowBoy.ContentApi.DTOs.Certifications;

namespace TheBabyCowBoy.ContentApi.Services.Interfaces;

public interface ICertificationService
{
    Task<List<CertificationResponseDto>> GetAllAsync();
    Task<CertificationResponseDto?> GetByIdAsync(Guid id);
    Task<CertificationResponseDto> CreateAsync(CertificationRequestDto request);
    Task<CertificationResponseDto?> UpdateAsync(Guid id, CertificationRequestDto request);
    Task<bool> DeleteAsync(Guid id);
}
