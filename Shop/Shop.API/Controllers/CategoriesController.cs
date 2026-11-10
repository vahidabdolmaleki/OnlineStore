using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.Categories;
using Shop.Application.Interfaces;

namespace Shop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    private readonly ICategoryService _categoryService = categoryService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(categories);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null)
            return NotFound(new { message = $"دسته‌بندی با شناسه {id} یافت نشد." });

        return Ok(category);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var id = await _categoryService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id }, new { id, message = "دسته‌بندی با موفقیت ایجاد شد." });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
    {
        if (id != dto.Id)
            return BadRequest(new { message = "عدم تطابق شناسه دسته‌بندی." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _categoryService.UpdateAsync(dto);
        if (!result)
            return NotFound(new { message = $"دسته‌بندی با شناسه {id} یافت نشد یا ویرایش ناموفق بود." });

        return Ok(new { message = "دسته‌بندی با موفقیت ویرایش شد." });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _categoryService.DeleteAsync(id);
        if (!result)
            return NotFound(new { message = $"دسته‌بندی با شناسه {id} یافت نشد یا حذف ناموفق بود." });

        return Ok(new { message = "دسته‌بندی با موفقیت غیرفعال (حذف منطقی) شد." });
    }
}
