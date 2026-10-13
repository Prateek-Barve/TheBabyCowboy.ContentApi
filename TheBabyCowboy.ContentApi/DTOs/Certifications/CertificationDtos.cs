
namespace TheBabyCowBoy.ContentApi.DTOs.Certifications;

public class CertificationRequestDto
{
    public string Name { get; set; } = string.Empty;

    public string Issuer { get; set; } = string.Empty;

    public string? Url { get; set; }
}

public class CertificationResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Issuer { get; set; } = string.Empty;

    public string? Url { get; set; }
}
