using Shop.Domain.Enums;

namespace Shop.Application.DTOs.ProductReviews;

public class ProductReviewDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Title { get; set; }
    public string? Comment { get; set; }
    public bool IsVerifiedPurchase { get; set; }
    public ReviewStatus Status { get; set; }
    public DateTime CreateDateUtc { get; set; }
}
