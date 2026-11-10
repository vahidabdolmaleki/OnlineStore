using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.Products;
using Shop.Application.Interfaces;

namespace Shop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound(new { message = $"محصول با شناسه {id} یافت نشد." });

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var id = await _productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id }, new { id, message = "محصول با موفقیت ثبت شد." });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
    {
        if (id != dto.Id)
            return BadRequest(new { message = "عدم تطابق شناسه محصول." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _productService.UpdateAsync(dto);
        if (!result)
            return NotFound(new { message = $"محصول با شناسه {id} یافت نشد یا ویرایش ناموفق بود." });

        return Ok(new { message = "محصول با موفقیت ویرایش شد." });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _productService.DeleteAsync(id);
        if (!result)
            return NotFound(new { message = $"محصول با شناسه {id} یافت نشد یا حذف ناموفق بود." });

        return Ok(new { message = "محصول با موفقیت غیرفعال (حذف منطقی) شد." });
    }
    [HttpGet("{productId:int}/details")]
    public async Task<IActionResult> GetDetails(int productId)
    {
        var details = await _productService.GetDetailsByProductIdAsync(productId);
        if (details == null)
            return NotFound(new { message = $"جزئیاتی برای محصول با شناسه {productId} ثبت نشده است." });

        return Ok(details);
    }

    [HttpPost("details")]
    public async Task<IActionResult> SaveDetails([FromBody] UpsertProductDetailsDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _productService.UpsertDetailsAsync(dto);
        if (!result)
            return BadRequest(new { message = "ثبت جزئیات با خطا مواجه شد. ممکن است شناسه محصول نامعتبر باشد." });

        return Ok(new { message = "جزئیات محصول با موفقیت ثبت/ویرایش شد." });
    }

}
