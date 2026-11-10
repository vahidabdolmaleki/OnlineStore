using Shop.Domain.Enums;

namespace Shop.Application.DTOs.ProductReviews;

public class UpdateProductReviewStatusDto
{
    public ReviewStatus Status { get; set; }
}
