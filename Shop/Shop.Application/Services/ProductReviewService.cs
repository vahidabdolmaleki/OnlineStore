using Shop.Application.DTOs.ProductReviews;
using Shop.Application.Interfaces;
using Shop.Domain.Entities;
using Shop.Domain.Enums;
using Shop.Domain.Interfaces;

namespace Shop.Application.Services;

public class ProductReviewService(IUnitOfWork unitOfWork)
    : IProductReviewService
{
    public async Task<IReadOnlyList<ProductReviewDto>>
        GetApprovedByProductIdAsync(int productId)
    {
        var product = await unitOfWork.Repository<Product>()
            .GetByIdAsync(productId);

        if (product is null || !product.IsActive)
            return Array.Empty<ProductReviewDto>();

        var reviews = await unitOfWork.Repository<ProductReview>()
            .GetAsync(r =>
                r.ProductId == productId &&
                r.IsActive &&
                r.Status == ReviewStatus.Approved);

        return await MapReviewsAsync(reviews);
    }

    public async Task<IReadOnlyList<ProductReviewDto>> GetPendingAsync()
    {
        var reviews = await unitOfWork.Repository<ProductReview>()
            .GetAsync(r =>
                r.IsActive &&
                r.Status == ReviewStatus.Pending);

        return await MapReviewsAsync(reviews);
    }

    public async Task<(bool Success, string Message, ProductReviewDto? Data)>
        CreateAsync(CreateProductReviewDto dto)
    {
        if (dto.Rating is < 1 or > 5)
            return (false, "امتیاز باید بین ۱ تا ۵ باشد.", null);

        if (string.IsNullOrWhiteSpace(dto.Comment))
            return (false, "متن نظر نمی‌تواند خالی باشد.", null);

        var product = await unitOfWork.Repository<Product>()
            .GetByIdAsync(dto.ProductId);

        if (product is null || !product.IsActive)
            return (false, "محصول یافت نشد.", null);

        var person = await unitOfWork.Repository<Person>()
            .GetByIdAsync(dto.PersonId);

        if (person is null || !person.IsActive)
            return (false, "شخص یافت نشد.", null);

        var reviewRepository = unitOfWork.Repository<ProductReview>();

        var existingReviews = await reviewRepository.GetAsync(r =>
            r.ProductId == dto.ProductId &&
            r.PersonId == dto.PersonId &&
            r.IsActive);

        if (existingReviews.Count > 0)
            return (false, "شما قبلاً برای این محصول نظر ثبت کرده‌اید.", null);

        var review = new ProductReview
        {
            ProductId = dto.ProductId,
            PersonId = dto.PersonId,
            Rating = dto.Rating,
            Title = string.IsNullOrWhiteSpace(dto.Title)
                ? null
                : dto.Title.Trim(),
            Comment = dto.Comment.Trim(),
            Status = ReviewStatus.Pending,
            IsVerifiedPurchase = false,
            IsActive = true
        };

        await reviewRepository.AddAsync(review);
        await unitOfWork.CompleteAsync();

        return (
            true,
            "نظر ثبت شد و پس از تأیید نمایش داده می‌شود.",
            MapReview(review, person)
        );
    }

    public async Task<(bool Success, string Message)> UpdateStatusAsync(
        int id,
        ReviewStatus status)
    {
        if (status is not (ReviewStatus.Approved or ReviewStatus.Rejected))
            return (false, "وضعیت باید Approved یا Rejected باشد.");

        var review = await unitOfWork.Repository<ProductReview>()
            .GetByIdAsync(id);

        if (review is null || !review.IsActive)
            return (false, "نظر یافت نشد.");

        if (review.Status != ReviewStatus.Pending)
            return (false, "فقط نظر در انتظار بررسی قابل تأیید یا رد است.");

        review.Status = status;
        review.EditDateUtc = DateTime.UtcNow;

        // GetByIdAsync در مخزن تو از FindAsync استفاده می‌کند؛
        // موجودیت Track می‌شود و نیازی به Update دوباره نیست.
        await unitOfWork.CompleteAsync();

        return (true, status == ReviewStatus.Approved
            ? "نظر تأیید شد."
            : "نظر رد شد.");
    }

    private async Task<IReadOnlyList<ProductReviewDto>> MapReviewsAsync(
        IReadOnlyList<ProductReview> reviews)
    {
        if (reviews.Count == 0)
            return Array.Empty<ProductReviewDto>();

        var personIds = reviews
            .Select(r => r.PersonId)
            .Distinct()
            .ToList();

        var people = await unitOfWork.Repository<Person>()
            .GetAsync(p => personIds.Contains(p.Id) && p.IsActive);

        var peopleById = people.ToDictionary(p => p.Id);

        return reviews
            .Where(r => peopleById.ContainsKey(r.PersonId))
            .OrderByDescending(r => r.CreateDateUtc)
            .Select(r => MapReview(r, peopleById[r.PersonId]))
            .ToList();
    }

    private static ProductReviewDto MapReview(
        ProductReview review,
        Person person)
    {
        return new ProductReviewDto
        {
            Id = review.Id,
            ProductId = review.ProductId,
            ReviewerName =
                $"{person.FirstName} {person.LastName}".Trim(),
            Rating = review.Rating,
            Title = review.Title,
            Comment = review.Comment,
            IsVerifiedPurchase = review.IsVerifiedPurchase,
            Status = review.Status,
            CreateDateUtc = review.CreateDateUtc
        };
    }
}
