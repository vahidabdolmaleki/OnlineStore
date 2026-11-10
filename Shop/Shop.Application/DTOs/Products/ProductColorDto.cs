namespace Shop.Application.DTOs.Products;

public class ProductColorDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ColorName { get; set; } = string.Empty;
    public string ColorCode { get; set; } = string.Empty;
    public List<ProductVariantDto> Variants { get; set; } = [];
}
