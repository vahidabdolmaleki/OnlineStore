namespace Shop.Application.DTOs.Products.Details;

public class ProductDetailsDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string? Brand { get; set; }
    public DateTime? ProductionDate { get; set; }
    public string? BatchNumber { get; set; }

    public DateTime? ExpireDate { get; set; }
    public string? ReturnConditions { get; set; }
    public string? WashingConditions { get; set; }
    public decimal? Height { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Radius { get; set; }
    public decimal? Weight { get; set; }
    public string? Material { get; set; }
    public string? CountryOfOrigin { get; set; }

}
