using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.Products.Details;
using Shop.Application.DTOs.Products.Products;
using Shop.Application.DTOs.Products.Variant;
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
        return CreatedAtAction(
            nameof(GetById),
            new { id },
            new { id, message = "محصول با موفقیت ثبت شد." });
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

    // جزئیات محصول

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

    // رنگ‌های محصول

    [HttpGet("{productId:int}/colors")]
    public async Task<IActionResult> GetColors(int productId)
    {
        var colors = await _productService.GetColorsByProductIdAsync(productId);
        return Ok(colors);
    }

    [HttpPost("colors")]
    public async Task<IActionResult> SaveColor([FromBody] UpsertProductColorDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _productService.UpsertColorAsync(dto);
        if (!result)
            return BadRequest(new { message = "ثبت یا ویرایش رنگ انجام نشد؛ اطلاعات ارسالی را بررسی کنید." });

        return Ok(new { message = "رنگ محصول با موفقیت ثبت/ویرایش شد." });
    }

    [HttpDelete("colors/{colorId:int}")]
    public async Task<IActionResult> DeleteColor(int colorId)
    {
        var result = await _productService.DeleteColorAsync(colorId);
        if (!result)
            return NotFound(new { message = $"رنگ با شناسه {colorId} یافت نشد یا حذف ناموفق بود." });

        return Ok(new { message = "رنگ محصول با موفقیت حذف شد." });
    }

    // واریانت‌های محصول

    [HttpGet("{productId:int}/variants")]
    public async Task<IActionResult> GetVariantsByProductId(int productId)
    {
        var variants = await _productService.GetVariantsByProductIdAsync(productId);
        return Ok(variants);
    }

    [HttpGet("colors/{colorId:int}/variants")]
    public async Task<IActionResult> GetVariantsByColorId(int colorId)
    {
        var variants = await _productService.GetVariantsByColorIdAsync(colorId);
        return Ok(variants);
    }

    [HttpPost("variants")]
    public async Task<IActionResult> SaveVariant([FromBody] UpsertProductVariantDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _productService.UpsertVariantAsync(dto);
        if (!result)
            return BadRequest(new { message = "ثبت یا ویرایش واریانت انجام نشد؛ اطلاعات ارسالی را بررسی کنید." });

        return Ok(new { message = "واریانت محصول با موفقیت ثبت/ویرایش شد." });
    }

    [HttpDelete("variants/{variantId:int}")]
    public async Task<IActionResult> DeleteVariant(int variantId)
    {
        var result = await _productService.DeleteVariantAsync(variantId);
        if (!result)
            return NotFound(new { message = $"واریانت با شناسه {variantId} یافت نشد یا حذف ناموفق بود." });

        return Ok(new { message = "واریانت محصول با موفقیت حذف شد." });
    }
}
