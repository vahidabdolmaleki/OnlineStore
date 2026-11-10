using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.Images;
using Shop.Application.Interfaces;

namespace Shop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductImagesController(
    IProductImageService imageService,
    IWebHostEnvironment env) : ControllerBase
{
    private readonly IProductImageService _imageService = imageService;
    private readonly IWebHostEnvironment _env = env;

    [HttpGet("product/{productId:int}")]
    public async Task<IActionResult> GetByProductId(int productId)
    {
        var images = await _imageService.GetByProductIdAsync(productId);
        return Ok(images);
    }

    [HttpGet("variant/{variantId:int}")]
    public async Task<IActionResult> GetByVariantId(int variantId)
    {
        var images = await _imageService.GetByVariantIdAsync(variantId);
        return Ok(images);
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] UploadProductImageDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        try
        {
            var result = await _imageService.UploadImageAsync(dto, webRootPath);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}/set-main")]
    public async Task<IActionResult> SetMain(int id)
    {
        var success = await _imageService.SetMainImageAsync(id);
        if (!success)
            return NotFound(new { message = "تصویر یافت نشد یا عملیات ناموفق بود." });

        return Ok(new { message = "تصویر به عنوان تصویر اصلی تنظیم شد." });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var success = await _imageService.DeleteImageAsync(id, webRootPath);
        if (!success)
            return NotFound(new { message = "تصویر یافت نشد." });

        return Ok(new { message = "تصویر با موفقیت حذف شد." });
    }
}
