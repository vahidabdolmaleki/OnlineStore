using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Shop.Application.DTOs.ProductReviews;
using Shop.Application.Interfaces;

namespace Shop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductReviewsController(
    IProductReviewService reviewService,
    IHostEnvironment environment) : ControllerBase
{
    [HttpGet("product/{productId:int}")]
    public async Task<IActionResult> GetByProductId(int productId)
    {
        if (productId < 1)
            return BadRequest(new { message = "شناسه محصول نامعتبر است." });

        var reviews =
            await reviewService.GetApprovedByProductIdAsync(productId);

        return Ok(reviews);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductReviewDto dto)
    {
        if (!environment.IsDevelopment())
            return StatusCode(403, new
            {
                message = "ثبت نظر تا زمان اتصال احراز هویت غیرفعال است."
            });

        var result = await reviewService.CreateAsync(dto);

        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return StatusCode(StatusCodes.Status201Created, result.Data);
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        if (!environment.IsDevelopment())
            return StatusCode(403, new
            {
                message = "مدیریت نظرات تا زمان اتصال دسترسی مدیر غیرفعال است."
            });

        return Ok(await reviewService.GetPendingAsync());
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody] UpdateProductReviewStatusDto dto)
    {
        if (!environment.IsDevelopment())
            return StatusCode(403, new
            {
                message = "مدیریت نظرات تا زمان اتصال دسترسی مدیر غیرفعال است."
            });

        var result = await reviewService.UpdateStatusAsync(id, dto.Status);

        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }
}
