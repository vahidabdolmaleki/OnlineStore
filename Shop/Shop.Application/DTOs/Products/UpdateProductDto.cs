namespace Shop.Application.DTOs.Products;

public class UpdateProductDto : CreateProductDto
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}
