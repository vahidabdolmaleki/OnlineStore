using Shop.Domain.Enums;

namespace Shop.Application.DTOs.ProductRelations;

public class ProductRelationDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int RelatedProductId { get; set; }
    public string RelatedProductName { get; set; } = string.Empty;
    public string? RelatedProductSlug { get; set; }
    public decimal RelatedProductPrice { get; set; }
    public decimal? RelatedProductDiscountPrice { get; set; }
    public string? RelatedProductMainImageUrl { get; set; }
    public ProductRelationType RelationType { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}
