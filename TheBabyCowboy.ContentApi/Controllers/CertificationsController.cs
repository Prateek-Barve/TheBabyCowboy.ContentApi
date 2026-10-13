
using Microsoft.AspNetCore.Mvc;
using TheBabyCowBoy.ContentApi.DTOs.Certifications;
using TheBabyCowBoy.ContentApi.Services.Interfaces;

namespace TheBabyCowBoy.ContentApi.Controllers;

[ApiController]
[Route("api/certifications")]
public class CertificationsController : ControllerBase
{
    private readonly ICertificationService _service;

    public CertificationsController(ICertificationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var certification = await _service.GetByIdAsync(id);

        return certification is null
            ? NotFound(new { message = "Certification not found." })
            : Ok(certification);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CertificationRequestDto request)
    {
        try
        {
            var certification = await _service.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = certification.Id },
                certification);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id, CertificationRequestDto request)
    {
        try
        {
            var certification = await _service.UpdateAsync(id, request);

            return certification is null
                ? NotFound(new { message = "Certification not found." })
                : Ok(certification);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);

        return deleted
            ? NoContent()
            : NotFound(new { message = "Certification not found." });
    }
}
