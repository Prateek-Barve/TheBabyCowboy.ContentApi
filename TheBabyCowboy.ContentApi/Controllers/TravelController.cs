
using Microsoft.AspNetCore.Mvc;
using TheBabyCowBoy.ContentApi.DTOs.Travel;
using TheBabyCowBoy.ContentApi.Services.Interfaces;

namespace TheBabyCowBoy.ContentApi.Controllers;

[ApiController]
[Route("api/travel")]
public class TravelController : ControllerBase
{
    private readonly ITravelService _service;

    public TravelController(ITravelService service)
    {
        _service = service;
    }

    // GET: api/travel
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    // GET: api/travel/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var trip = await _service.GetByIdAsync(id);

        return trip is null
            ? NotFound(new { message = "Travel trip not found." })
            : Ok(trip);
    }

    // POST: api/travel
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] TravelTripRequestDto request)
    {
        try
        {
            var trip = await _service.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = trip.Id },
                trip);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/travel/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] TravelTripRequestDto request)
    {
        try
        {
            var trip = await _service.UpdateAsync(id, request);

            return trip is null
                ? NotFound(new { message = "Travel trip not found." })
                : Ok(trip);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE: api/travel/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);

        return deleted
            ? NoContent()
            : NotFound(new { message = "Travel trip not found." });
    }

    // POST: api/travel/{id}/photos
    [HttpPost("{id:guid}/photos")]
    public async Task<IActionResult> AddPhoto(
        Guid id,
        [FromBody] TravelPhotoRequestDto request)
    {
        try
        {
            var photo = await _service.AddPhotoAsync(id, request);

            if (photo is null)
                return NotFound(
                    new { message = "Travel trip not found." });

            return Ok(photo);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE: api/travel/{id}/photos/{photoId}
    [HttpDelete("{id:guid}/photos/{photoId:guid}")]
    public async Task<IActionResult> DeletePhoto(
        Guid id,
        Guid photoId)
    {
        var deleted = await _service.DeletePhotoAsync(id, photoId);

        return deleted
            ? NoContent()
            : NotFound(new
            {
                message = "Travel trip or photo not found."
            });
    }
}
