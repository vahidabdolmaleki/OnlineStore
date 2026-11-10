using Shop.Application.DTOs.ProductRelations;
using Shop.Application.Interfaces;
using Shop.Domain.Entities;
using Shop.Domain.Enums;
using Shop.Domain.Interfaces;

namespace Shop.Application.Services;

public class ProductRelationService(IUnitOfWork unitOfWork)
    : IProductRelationService
{
    public async Task<IEnumerable<ProductRelationDto>> GetByProductIdAsync(
        int productId,
        ProductRelationType? relationType = null)
    {
        var relationRepo = unitOfWork.Repository<ProductRelation>();
        var productRepo = unitOfWork.Repository<Product>();
        var imageRepo = unitOfWork.Repository<Images>();

        var relations = await relationRepo.GetAsync(r =>
            r.ProductId == productId &&
            r.IsActive &&
            (!relationType.HasValue || r.RelationType == relationType.Value));

        var orderedRelations = relations
            .OrderBy(r => r.SortOrder)
            .ToList();

        if (orderedRelations.Count == 0)
            return Enumerable.Empty<ProductRelationDto>();

        var relatedProductIds = orderedRelations
            .Select(r => r.RelatedProductId)
            .Distinct()
            .ToList();

        var products = await productRepo.GetAsync(p =>
            relatedProductIds.Contains(p.Id) && p.IsActive);

        var productById = products.ToDictionary(p => p.Id);

        var images = await imageRepo.GetAsync(img =>
            img.ProductId.HasValue &&
            relatedProductIds.Contains(img.ProductId.Value) &&
            img.IsMain &&
            img.IsActive);

        var imageByProductId = images
            .GroupBy(img => img.ProductId!.Value)
            .ToDictionary(g => g.Key, g => g.First().Url);

        return orderedRelations
            .Where(r => productById.ContainsKey(r.RelatedProductId))
            .Select(r =>
            {
                var product = productById[r.RelatedProductId];

                imageByProductId.TryGetValue(
                    r.RelatedProductId,
                    out var imageUrl);

                return MapToDto(r, product, imageUrl);
            })
            .ToList();
    }

    public async Task<ProductRelationDto?> GetByIdAsync(int id)
    {
        var relationRepo = unitOfWork.Repository<ProductRelation>();
        var productRepo = unitOfWork.Repository<Product>();
        var imageRepo = unitOfWork.Repository<Images>();

        var relation = await relationRepo.GetByIdAsync(id);

        if (relation is null || !relation.IsActive)
            return null;

        var product = await productRepo.GetByIdAsync(relation.RelatedProductId);

        if (product is null || !product.IsActive)
            return null;

        var images = await imageRepo.GetAsync(img =>
            img.ProductId == relation.RelatedProductId &&
            img.IsMain &&
            img.IsActive);

        var imageUrl = images.FirstOrDefault()?.Url;

        return MapToDto(relation, product, imageUrl);
    }

    public async Task<(bool Success, string Message, ProductRelationDto? Data)> AddAsync(
        CreateProductRelationDto dto)
    {
        if (dto.ProductId == dto.RelatedProductId)
            return (false, "یک محصول نمی‌تواند با خودش مرتبط شود.", null);

        var productRepo = unitOfWork.Repository<Product>();
        var relationRepo = unitOfWork.Repository<ProductRelation>();

        var product = await productRepo.GetByIdAsync(dto.ProductId);
        if (product is null || !product.IsActive)
            return (false, "محصول اصلی یافت نشد.", null);

        var relatedProduct = await productRepo.GetByIdAsync(dto.RelatedProductId);
        if (relatedProduct is null || !relatedProduct.IsActive)
            return (false, "محصول مرتبط یافت نشد.", null);

        var duplicateRelations = await relationRepo.GetAsync(r =>
            r.ProductId == dto.ProductId &&
            r.RelatedProductId == dto.RelatedProductId &&
            r.RelationType == dto.RelationType &&
            r.IsActive);

        if (duplicateRelations.Count > 0)
            return (false, "این رابطه قبلاً ثبت شده است.", null);

        var relation = new ProductRelation
        {
            ProductId = dto.ProductId,
            RelatedProductId = dto.RelatedProductId,
            RelationType = dto.RelationType,
            SortOrder = dto.SortOrder,
            CreateDateUtc = DateTime.UtcNow,
            IsActive = true
        };

        await relationRepo.AddAsync(relation);
        await unitOfWork.CompleteAsync();

        var result = await GetByIdAsync(relation.Id);

        return (true, "رابطه محصول با موفقیت ایجاد شد.", result);
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        var relationRepo = unitOfWork.Repository<ProductRelation>();

        // FindAsync موجودیت را Track می‌کند؛ برای تغییر IsActive مناسب است.
        var relation = await relationRepo.GetByIdAsync(id);

        if (relation is null || !relation.IsActive)
            return (false, "رابطه مورد نظر یافت نشد.");

        relation.IsActive = false;
        relation.EditDateUtc = DateTime.UtcNow;

        relationRepo.Update(relation);
        await unitOfWork.CompleteAsync();

        return (true, "رابطه با موفقیت حذف شد.");
    }

    private static ProductRelationDto MapToDto(
        ProductRelation relation,
        Product product,
        string? imageUrl)
    {
        return new ProductRelationDto
        {
            Id = relation.Id,
            ProductId = relation.ProductId,
            RelatedProductId = relation.RelatedProductId,
            RelatedProductName = product.Name,
            RelatedProductSlug = product.Slug,
            RelatedProductPrice = product.Price,
            RelatedProductDiscountPrice = product.DiscountPrice,
            RelatedProductMainImageUrl = imageUrl,
            RelationType = relation.RelationType,
            SortOrder = relation.SortOrder,
            IsActive = relation.IsActive
        };
    }
}
