using Shop.Application.DTOs.Categories;

namespace Shop.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateCategoryDto dto);
    Task<bool> UpdateAsync(UpdateCategoryDto dto);
    Task<bool> DeleteAsync(int id);
}
