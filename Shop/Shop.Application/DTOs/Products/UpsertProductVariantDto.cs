namespace Shop.Application.DTOs.Products;

public class UpsertProductVariantDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int? ProductColorId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string? VariantName { get; set; }
    public string? Size { get; set; }
    public decimal? PriceOverride { get; set; }
    public decimal? Weight { get; set; }
    public bool IsDefault { get; set; }
}
