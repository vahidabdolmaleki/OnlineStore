using Shop.Application.DTOs.ProductReviews;
using Shop.Domain.Enums;

namespace Shop.Application.Interfaces;

public interface IProductReviewService
{
    Task<IReadOnlyList<ProductReviewDto>> GetApprovedByProductIdAsync(int productId);
    Task<IReadOnlyList<ProductReviewDto>> GetPendingAsync();

    Task<(bool Success, string Message, ProductReviewDto? Data)> CreateAsync(
        CreateProductReviewDto dto);

    Task<(bool Success, string Message)> UpdateStatusAsync(
        int id,
        ReviewStatus status);
}
