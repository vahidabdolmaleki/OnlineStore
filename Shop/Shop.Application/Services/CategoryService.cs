using Shop.Application.DTOs.Categories;
using Shop.Application.Interfaces;
using Shop.Domain.Entities;
using Shop.Domain.Interfaces;

namespace Shop.Application.Services;

public class CategoryService(IUnitOfWork unitOfWork) : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _unitOfWork.Repository<Category>().GetAllAsync();
        var categoryDict = categories.ToDictionary(c => c.Id, c => c.Title);

        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Title = c.Title,
            Slug = c.Slug,
            Description = c.Description,
            ParentId = c.ParentId,
            ParentTitle = c.ParentId.HasValue && categoryDict.TryGetValue(c.ParentId.Value, out var parentTitle)
                ? parentTitle
                : null,
            SortOrder = c.SortOrder,
            MetaTitle = c.MetaTitle,
            MetaDescription = c.MetaDescription,
            CreateDateUtc = c.CreateDateUtc
        });
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
        if (category == null) return null;

        string? parentTitle = null;
        if (category.ParentId.HasValue)
        {
            var parent = await _unitOfWork.Repository<Category>().GetByIdAsync(category.ParentId.Value);
            parentTitle = parent?.Title;
        }

        return new CategoryDto
        {
            Id = category.Id,
            Title = category.Title,
            Slug = category.Slug,
            Description = category.Description,
            ParentId = category.ParentId,
            ParentTitle = parentTitle,
            SortOrder = category.SortOrder,
            MetaTitle = category.MetaTitle,
            MetaDescription = category.MetaDescription,
            CreateDateUtc = category.CreateDateUtc
        };
    }

    public async Task<int> CreateAsync(CreateCategoryDto dto)
    {
        var category = new Category
        {
            Title = dto.Title,
            Slug = dto.Slug,
            Description = dto.Description,
            ParentId = dto.ParentId,
            SortOrder = dto.SortOrder,
            MetaTitle = dto.MetaTitle,
            MetaDescription = dto.MetaDescription,
            IsActive = true
        };

        await _unitOfWork.Repository<Category>().AddAsync(category);
        await _unitOfWork.CompleteAsync();

        return category.Id;
    }

    public async Task<bool> UpdateAsync(UpdateCategoryDto dto)
    {
        var category = await _unitOfWork.Repository<Category>().GetByIdAsync(dto.Id);
        if (category == null) return false;

        category.Title = dto.Title;
        category.Slug = dto.Slug;
        category.Description = dto.Description;
        category.ParentId = dto.ParentId;
        category.SortOrder = dto.SortOrder;
        category.MetaTitle = dto.MetaTitle;
        category.MetaDescription = dto.MetaDescription;
        category.IsActive = dto.IsActive;
        category.EditDateUtc = DateTime.UtcNow;

        _unitOfWork.Repository<Category>().Update(category);
        return await _unitOfWork.CompleteAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
        if (category == null) return false;

        _unitOfWork.Repository<Category>().Delete(category);
        return await _unitOfWork.CompleteAsync() > 0;
    }
}
