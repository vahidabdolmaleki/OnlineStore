using Shop.Application.DTOs.Images;
using Shop.Application.DTOs.Products;


namespace Shop.Application.Interfaces;

public interface IProductImageService
{
    Task<IEnumerable<ProductImageDto>> GetByProductIdAsync(int productId);
    Task<IEnumerable<ProductImageDto>> GetByVariantIdAsync(int variantId);
    Task<ProductImageDto?> GetByIdAsync(int imageId);
    Task<ProductImageDto> UploadImageAsync(UploadProductImageDto dto, string webRootPath);
    Task<bool> SetMainImageAsync(int imageId);
    Task<bool> DeleteImageAsync(int imageId, string webRootPath);
}
