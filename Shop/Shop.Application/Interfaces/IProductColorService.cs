using Shop.Application.DTOs.ProductColors;
using Shop.Application.DTOs.Products.Color;

namespace Shop.Application.Interfaces;

public interface IProductColorService
{
    Task<IReadOnlyList<ProductColorDto>> GetByProductIdAsync(int productId);
    Task<ProductColorDto?> GetByIdAsync(int id);
    Task<(bool Success, string Message, ProductColorDto? Data)> CreateAsync(CreateProductColorDto dto);
    Task<(bool Success, string Message)> UpdateAsync(int id, UpdateProductColorDto dto);
    Task<(bool Success, string Message)> DeleteAsync(int id);
}
