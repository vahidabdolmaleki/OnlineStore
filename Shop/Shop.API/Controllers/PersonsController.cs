using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.Person;
using Shop.Application.Interfaces;

namespace Shop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonsController(IPersonService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var person = await service.GetByIdAsync(id);
        return person is null ? NotFound() : Ok(person);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePersonDto dto)
    {
        try { return Ok(await service.CreateAsync(dto)); }
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePersonDto dto)
    {
        var person = await service.UpdateAsync(id, dto);
        return person is null ? NotFound() : Ok(person);
    }
    /// <summary>
    /// حذف نرم کاربر (IsActive = false)
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await service.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
