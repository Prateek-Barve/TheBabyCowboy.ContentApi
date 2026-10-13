
namespace TheBabyCowboy.ContentApi.Models;

public class Certification
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string Issuer { get; set; } = string.Empty;

    public string? Url { get; set; }
}
