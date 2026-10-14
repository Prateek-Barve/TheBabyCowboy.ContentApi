
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheBabyCowBoy.ContentApi.DTOs.Skills;
using TheBabyCowBoy.ContentApi.Services.Interfaces;

namespace TheBabyCowBoy.ContentApi.Controllers;

[ApiController]
[Route("api/skills")]
public class SkillsController : ControllerBase
{
    private readonly ISkillService _service;

    public SkillsController(ISkillService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? categoryId)
        => Ok(await _service.GetAllAsync(categoryId));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var skill = await _service.GetByIdAsync(id);

        return skill is null
            ? NotFound(new { message = "Skill not found." })
            : Ok(skill);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create(SkillRequestDto request)
    {
        try
        {
            var skill = await _service.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = skill.Id },
                skill);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(
        Guid id, SkillRequestDto request)
    {
        try
        {
            var skill = await _service.UpdateAsync(id, request);

            return skill is null
                ? NotFound(new { message = "Skill not found." })
                : Ok(skill);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);

        return deleted
            ? NoContent()
            : NotFound(new { message = "Skill not found." });
    }
}
