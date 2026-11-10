namespace Shop.Application.DTOs.Products;

public class ProductVariantDto
{
    public int Id { get; set; }
    public int ProductColorId { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? Sku { get; set; } // کد انبار / بارکد
}
