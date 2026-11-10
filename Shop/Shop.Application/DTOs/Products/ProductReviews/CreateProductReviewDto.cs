using System.ComponentModel.DataAnnotations;

namespace Shop.Application.DTOs.ProductReviews;

public class CreateProductReviewDto
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    // موقتاً برای تست Development؛ بعداً از JWT گرفته می‌شود.
    [Range(1, int.MaxValue)]
    public int PersonId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [StringLength(150)]
    public string? Title { get; set; }

    [Required]
    [StringLength(2000)]
    public string Comment { get; set; } = string.Empty;
}
