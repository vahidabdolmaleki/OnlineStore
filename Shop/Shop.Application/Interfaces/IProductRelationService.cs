using Shop.Application.DTOs.ProductRelations;
using Shop.Domain.Enums;

namespace Shop.Application.Interfaces;

public interface IProductRelationService
{
    Task<IEnumerable<ProductRelationDto>> GetByProductIdAsync(int productId, ProductRelationType? relationType = null);
    Task<ProductRelationDto?> GetByIdAsync(int id);
    Task<(bool Success, string Message, ProductRelationDto? Data)> AddAsync(CreateProductRelationDto dto);
    Task<(bool Success, string Message)> DeleteAsync(int id);
}
