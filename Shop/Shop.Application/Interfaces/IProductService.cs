using Shop.Application.DTOs.Products;

namespace Shop.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateProductDto dto);
    Task<bool> UpdateAsync(UpdateProductDto dto);
    Task<bool> DeleteAsync(int id);

    Task<ProductDetailsDto?> GetDetailsByProductIdAsync(int productId);
    Task<bool> UpsertDetailsAsync(UpsertProductDetailsDto dto);

    Task<IEnumerable<ProductColorDto>> GetColorsByProductIdAsync(int productId);
    Task<bool> UpsertColorAsync(UpsertProductColorDto dto);
    Task<bool> DeleteColorAsync(int colorId);

    Task<IEnumerable<ProductVariantDto>> GetVariantsByProductIdAsync(int productId);
    Task<IEnumerable<ProductVariantDto>> GetVariantsByColorIdAsync(int colorId); // اضافه شد
    Task<bool> UpsertVariantAsync(UpsertProductVariantDto dto);
    Task<bool> DeleteVariantAsync(int variantId);


}
