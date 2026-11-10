namespace Shop.Application.DTOs.Products.Products;

public class UpdateProductDto : CreateProductDto
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}
