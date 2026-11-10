namespace Shop.Application.DTOs.Products.Products;

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int CategoryId { get; set; }
    public bool HasVariants { get; set; }
    public bool IsFeatured { get; set; }
}
