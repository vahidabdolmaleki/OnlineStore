using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.ProductRelations;
using Shop.Application.Interfaces;
using Shop.Domain.Enums;

namespace Shop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductRelationsController(IProductRelationService relationService) : ControllerBase
{
    /// <summary>
    /// دریافت لیست محصولات مرتبط بر اساس شناسه محصول (با امکان فیلتر بر اساس نوع رابطه)
    /// </summary>
    [HttpGet("product/{productId:int}")]
    public async Task<IActionResult> GetByProductId(int productId, [FromQuery] ProductRelationType? relationType)
    {
        var relations = await relationService.GetByProductIdAsync(productId, relationType);
        return Ok(relations);
    }

    /// <summary>
    /// دریافت جزئیات یک رابطه بر اساس شناسه
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var relation = await relationService.GetByIdAsync(id);
        if (relation == null)
            return NotFound(new { message = "رابطه مورد نظر یافت نشد." });

        return Ok(relation);
    }

    /// <summary>
    /// ثبت یک رابطه جدید بین دو محصول
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRelationDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await relationService.AddAsync(dto);
        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>
    /// حذف یک رابطه محصول (Soft Delete)
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await relationService.DeleteAsync(id);
        if (!result.Success)
            return NotFound(new { message = result.Message });

        return Ok(new { message = result.Message });
    }
}
