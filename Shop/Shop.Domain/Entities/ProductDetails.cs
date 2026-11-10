using Shop.Domain.Common;

public class ProductDetails : BaseEntity
{
    public int ProductId { get; set; }
    public string? Brand { get; set; }

    // تاریخ‌ها و سری ساخت
    public DateTime? ProductionDate { get; set; } // جدید
    public DateTime? ExpireDate { get; set; }
    public string? BatchNumber { get; set; }        // جدید (شماره سری ساخت)

    public string? ReturnConditions { get; set; }
    public string? WashingConditions { get; set; }
    public decimal? Height { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Radius { get; set; }
    public decimal? Weight { get; set; }
    public string? Material { get; set; }
    public string? CountryOfOrigin { get; set; }

    public Product Product { get; set; } = null!;
}
