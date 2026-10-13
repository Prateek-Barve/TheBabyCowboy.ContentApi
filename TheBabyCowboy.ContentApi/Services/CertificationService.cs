
using Microsoft.EntityFrameworkCore;
using TheBabyCowboy.ContentApi.Data;
using TheBabyCowboy.ContentApi.Models;
using TheBabyCowBoy.ContentApi.DTOs.Certifications;
using TheBabyCowBoy.ContentApi.Services.Interfaces;

namespace TheBabyCowBoy.ContentApi.Services;

public class CertificationService : ICertificationService
{
    private readonly ContentDbContext _context;

    public CertificationService(ContentDbContext context)
    {
        _context = context;
    }

    public async Task<List<CertificationResponseDto>> GetAllAsync()
    {
        var certifications = await _context.Certifications
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();

        return certifications.Select(MapToDto).ToList();
    }

    public async Task<CertificationResponseDto?> GetByIdAsync(Guid id)
    {
        var certification = await _context.Certifications
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        return certification is null ? null : MapToDto(certification);
    }

    public async Task<CertificationResponseDto> CreateAsync(
        CertificationRequestDto request)
    {
        ValidateRequest(request);

        var certification = new Certification
        {
            Name = request.Name.Trim(),
            Issuer = request.Issuer.Trim(),
            Url = NormalizeUrl(request.Url)
        };

        _context.Certifications.Add(certification);
        await _context.SaveChangesAsync();

        return MapToDto(certification);
    }

    public async Task<CertificationResponseDto?> UpdateAsync(
        Guid id, CertificationRequestDto request)
    {
        var certification = await _context.Certifications
            .FirstOrDefaultAsync(c => c.Id == id);

        if (certification is null)
            return null;

        ValidateRequest(request);

        certification.Name = request.Name.Trim();
        certification.Issuer = request.Issuer.Trim();
        certification.Url = NormalizeUrl(request.Url);

        await _context.SaveChangesAsync();

        return MapToDto(certification);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var certification = await _context.Certifications
            .FirstOrDefaultAsync(c => c.Id == id);

        if (certification is null)
            return false;

        _context.Certifications.Remove(certification);
        await _context.SaveChangesAsync();

        return true;
    }

    private static void ValidateRequest(CertificationRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Certification name is required.");

        if (request.Name.Trim().Length > 200)
            throw new ArgumentException(
                "Certification name cannot exceed 200 characters.");

        if (string.IsNullOrWhiteSpace(request.Issuer))
            throw new ArgumentException("Certification issuer is required.");

        if (request.Issuer.Trim().Length > 200)
            throw new ArgumentException(
                "Certification issuer cannot exceed 200 characters.");

        var url = NormalizeUrl(request.Url);

        if (url is not null &&
            (!Uri.TryCreate(url, UriKind.Absolute, out var parsedUri) ||
             (parsedUri.Scheme != Uri.UriSchemeHttp &&
              parsedUri.Scheme != Uri.UriSchemeHttps)))
        {
            throw new ArgumentException(
                "Certification URL must be a valid HTTP or HTTPS URL.");
        }
    }

    private static string? NormalizeUrl(string? url)
    {
        return string.IsNullOrWhiteSpace(url) ? null : url.Trim();
    }

    private static CertificationResponseDto MapToDto(
        Certification certification)
    {
        return new CertificationResponseDto
        {
            Id = certification.Id,
            Name = certification.Name,
            Issuer = certification.Issuer,
            Url = certification.Url
        };
    }
}
