using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.ProductColors;
using Shop.Application.Interfaces;

namespace Shop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductColorsController(IProductColorService colorService) : ControllerBase
{
    [HttpGet("product/{productId:int}")]
    public async Task<IActionResult> GetByProductId(int productId)
    {
        if (productId < 1)
            return BadRequest(new { message = "شناسه محصول نامعتبر است." });

        var colors = await colorService.GetByProductIdAsync(productId);
        return Ok(colors);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (id < 1)
            return BadRequest(new { message = "شناسه نامعتبر است." });

        var color = await colorService.GetByIdAsync(id);
        if (color is null)
            return NotFound(new { message = "رنگ محصول یافت نشد." });

        return Ok(color);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductColorDto dto)
    {
        var result = await colorService.CreateAsync(dto);
        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return StatusCode(StatusCodes.Status201Created, result.Data);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductColorDto dto)
    {
        var result = await colorService.UpdateAsync(id, dto);
        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await colorService.DeleteAsync(id);
        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }
}
