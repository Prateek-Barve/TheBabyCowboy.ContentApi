
using Microsoft.AspNetCore.Mvc;
using TheBabyCowBoy.ContentApi.DTOs.Blogs;
using TheBabyCowBoy.ContentApi.Services.Interfaces;

namespace TheBabyCowBoy.ContentApi.Controllers;

[ApiController]
[Route("api/blogs")]
public class BlogsController : ControllerBase
{
    private readonly IBlogService _blogService;

    public BlogsController(IBlogService blogService)
    {
        _blogService = blogService;
    }

    [HttpGet]
    public async Task<ActionResult<List<BlogResponseDto>>> GetAll()
    {
        var blogs = await _blogService.GetAllAsync();

        return Ok(blogs);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BlogResponseDto>> GetById(
        Guid id)
    {
        var blog = await _blogService.GetByIdAsync(id);

        if (blog is null)
        {
            return NotFound();
        }

        return Ok(blog);
    }

    [HttpPost]
    public async Task<ActionResult<BlogResponseDto>> Create(
        [FromBody] BlogRequestDto request)
    {
        try
        {
            var blog = await _blogService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = blog.Id },
                blog);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BlogResponseDto>> Update(
        Guid id,
        [FromBody] BlogRequestDto request)
    {
        try
        {
            var blog = await _blogService.UpdateAsync(id, request);

            if (blog is null)
            {
                return NotFound();
            }

            return Ok(blog);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _blogService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
