using Shop.Application.DTOs.ProductColors;
using Shop.Application.DTOs.Products.Color;
using Shop.Application.Interfaces;
using Shop.Domain.Entities;
using Shop.Domain.Interfaces;

namespace Shop.Application.Services;

public class ProductColorService(IUnitOfWork unitOfWork) : IProductColorService
{
    public async Task<IReadOnlyList<ProductColorDto>> GetByProductIdAsync(int productId)
    {
        var product = await unitOfWork.Repository<Product>().GetByIdAsync(productId);
        if (product is null || !product.IsActive)
            return Array.Empty<ProductColorDto>();

        var colors = await unitOfWork.Repository<ProductColor>()
            .GetAsync(c => c.ProductId == productId && c.IsActive);

        return colors.Select(MapToDto).ToList();
    }

    public async Task<ProductColorDto?> GetByIdAsync(int id)
    {
        var color = await unitOfWork.Repository<ProductColor>().GetByIdAsync(id);
        if (color is null || !color.IsActive)
            return null;

        return MapToDto(color);
    }

    public async Task<(bool Success, string Message, ProductColorDto? Data)> CreateAsync(CreateProductColorDto dto)
    {
        var product = await unitOfWork.Repository<Product>().GetByIdAsync(dto.ProductId);
        if (product is null || !product.IsActive)
            return (false, "محصول مورد نظر یافت نشد.", null);

        var trimmedName = dto.ColorName.Trim();
        var trimmedCode = dto.ColorCode.Trim();

        // جلوگیری از ثبت رنگ تکراری برای یک محصول
        var existing = await unitOfWork.Repository<ProductColor>()
            .GetAsync(c => c.ProductId == dto.ProductId &&
                           c.IsActive &&
                           (c.ColorName.ToLower() == trimmedName.ToLower() ||
                            c.ColorCode.ToLower() == trimmedCode.ToLower()));

        if (existing.Count > 0)
            return (false, "رنگی با این نام یا کد قبلاً برای این محصول ثبت شده است.", null);

        var color = new ProductColor
        {
            ProductId = dto.ProductId,
            ColorName = trimmedName,
            ColorCode = trimmedCode,
            IsActive = true
        };

        await unitOfWork.Repository<ProductColor>().AddAsync(color);
        await unitOfWork.CompleteAsync();

        return (true, "رنگ محصول با موفقیت ایجاد شد.", MapToDto(color));
    }

    public async Task<(bool Success, string Message)> UpdateAsync(int id, UpdateProductColorDto dto)
    {
        var color = await unitOfWork.Repository<ProductColor>().GetByIdAsync(id);
        if (color is null || !color.IsActive)
            return (false, "رنگ محصول یافت نشد.");

        var trimmedName = dto.ColorName.Trim();
        var trimmedCode = dto.ColorCode.Trim();

        // بررسی عدم تکراری بودن نام یا کد با سایر رنگ‌های همین محصول
        var duplicate = await unitOfWork.Repository<ProductColor>()
            .GetAsync(c => c.ProductId == color.ProductId &&
                           c.Id != id &&
                           c.IsActive &&
                           (c.ColorName.ToLower() == trimmedName.ToLower() ||
                            c.ColorCode.ToLower() == trimmedCode.ToLower()));

        if (duplicate.Count > 0)
            return (false, "رنگ دیگری با این نام یا کد برای این محصول وجود دارد.");

        color.ColorName = trimmedName;
        color.ColorCode = trimmedCode;
        color.EditDateUtc = DateTime.UtcNow;

        await unitOfWork.CompleteAsync();
        return (true, "رنگ محصول با موفقیت ویرایش شد.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        var color = await unitOfWork.Repository<ProductColor>().GetByIdAsync(id);
        if (color is null || !color.IsActive)
            return (false, "رنگ محصول یافت نشد.");

        // بررسی اینکه آیا تنوع فعالی به این رنگ وابسته است یا خیر
        var variantsUsingColor = await unitOfWork.Repository<ProductVariant>()
            .GetAsync(v => v.ProductColorId == id && v.IsActive);

        if (variantsUsingColor.Count > 0)
            return (false, "امکان حذف این رنگ وجود ندارد زیرا تنوع‌هایی به آن متصل هستند.");

        // سافت‌دیلیت با مکانیزم پروژه
        unitOfWork.Repository<ProductColor>().Delete(color);
        await unitOfWork.CompleteAsync();

        return (true, "رنگ محصول با موفقیت حذف شد.");
    }

    private static ProductColorDto MapToDto(ProductColor color)
    {
        return new ProductColorDto
        {
            Id = color.Id,
            ProductId = color.ProductId,
            ColorName = color.ColorName,
            ColorCode = color.ColorCode,
        };
    }
}
