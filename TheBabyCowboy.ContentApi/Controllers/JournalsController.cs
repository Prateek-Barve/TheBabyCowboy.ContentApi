
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheBabyCowBoy.ContentApi.DTOs.Journals;
using TheBabyCowBoy.ContentApi.Services.Interfaces;

namespace TheBabyCowBoy.ContentApi.Controllers;

[ApiController]
[Route("api/journals")]
public class JournalsController : ControllerBase
{
    private readonly IJournalService _journalService;

    public JournalsController(IJournalService journalService)
    {
        _journalService = journalService;
    }

    // Public: published/public entries only.
    [AllowAnonymous]
    [HttpGet("public")]
    public async Task<IActionResult> GetPublic(
        [FromQuery] string? search,
        [FromQuery] string? mood,
        [FromQuery] string? category,
        [FromQuery] string? tag,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to)
    {
        try
        {
            var entries = await _journalService.GetAllAsync(
                includePrivate: false,
                search, mood, category, tag, from, to);

            return Ok(entries);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Public: only an entry marked IsPublic can be returned.
    [AllowAnonymous]
    [HttpGet("public/{id:guid}")]
    public async Task<IActionResult> GetPublicById(Guid id)
    {
        var entry = await _journalService.GetByIdAsync(
            id, includePrivate: false);

        return entry is null ? NotFound() : Ok(entry);
    }

    // The remaining endpoints require authentication.
    // Configure an authentication scheme in Program.cs before using them.

    
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? mood,
        [FromQuery] string? category,
        [FromQuery] string? tag,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to)
    {
        try
        {
            var entries = await _journalService.GetAllAsync(
                includePrivate: true,
                search, mood, category, tag, from, to);

            return Ok(entries);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entry = await _journalService.GetByIdAsync(
            id, includePrivate: true);

        return entry is null ? NotFound() : Ok(entry);
    }

    
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] JournalRequestDto request)
    {
        try
        {
            var entry = await _journalService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = entry.Id },
                entry);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] JournalRequestDto request)
    {
        try
        {
            var entry = await _journalService.UpdateAsync(id, request);

            return entry is null ? NotFound() : Ok(entry);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _journalService.DeleteAsync(id);

        return deleted ? NoContent() : NotFound();
    }
}
