using Shop.Application.DTOs.Products.Color;
using Shop.Application.DTOs.Products.Details;
using Shop.Application.DTOs.Products.Products;
using Shop.Application.DTOs.Products.Variant;
using Shop.Application.Interfaces;
using Shop.Domain.Entities;
using Shop.Domain.Interfaces;

namespace Shop.Application.Services;

public class ProductService(IUnitOfWork unitOfWork) : IProductService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _unitOfWork.Repository<Product>().GetAllAsync();
        var categories = await _unitOfWork.Repository<Category>().GetAllAsync();
        var categoryDict = categories.ToDictionary(c => c.Id, c => c.Title);

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Code = p.Code,
            Slug = p.Slug,
            Description = p.Description,
            Price = p.Price,
            DiscountPrice = p.DiscountPrice,
            CategoryId = p.CategoryId,
            CategoryName = categoryDict.TryGetValue(p.CategoryId, out var catName) ? catName : string.Empty,
            HasVariants = p.HasVariants,
            IsFeatured = p.IsFeatured,
            ViewCount = p.ViewCount,
            CreateDateUtc = p.CreateDateUtc
        });
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
        if (product == null) return null;

        var category = await _unitOfWork.Repository<Category>().GetByIdAsync(product.CategoryId);

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Code = product.Code,
            Slug = product.Slug,
            Description = product.Description,
            Price = product.Price,
            DiscountPrice = product.DiscountPrice,
            CategoryId = product.CategoryId,
            CategoryName = category?.Title ?? string.Empty,
            HasVariants = product.HasVariants,
            IsFeatured = product.IsFeatured,
            ViewCount = product.ViewCount,
            CreateDateUtc = product.CreateDateUtc
        };
    }

    public async Task<int> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Code = dto.Code,
            Slug = dto.Slug,
            Description = dto.Description,
            Price = dto.Price,
            DiscountPrice = dto.DiscountPrice,
            CategoryId = dto.CategoryId,
            HasVariants = dto.HasVariants,
            IsFeatured = dto.IsFeatured,
            ViewCount = 0,
            IsActive = true
        };

        await _unitOfWork.Repository<Product>().AddAsync(product);
        await _unitOfWork.CompleteAsync();

        return product.Id;
    }

    public async Task<bool> UpdateAsync(UpdateProductDto dto)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(dto.Id);
        if (product == null) return false;

        product.Name = dto.Name;
        product.Code = dto.Code;
        product.Slug = dto.Slug;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.DiscountPrice = dto.DiscountPrice;
        product.CategoryId = dto.CategoryId;
        product.HasVariants = dto.HasVariants;
        product.IsFeatured = dto.IsFeatured;
        product.IsActive = dto.IsActive;
        product.EditDateUtc = DateTime.UtcNow;

        _unitOfWork.Repository<Product>().Update(product);
        return await _unitOfWork.CompleteAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
        if (product == null) return false;

        _unitOfWork.Repository<Product>().Delete(product);
        return await _unitOfWork.CompleteAsync() > 0;
    }

    public async Task<ProductDetailsDto?> GetDetailsByProductIdAsync(int productId)
    {
        var detailsList = await _unitOfWork.Repository<ProductDetails>().GetAllAsync();
        var details = detailsList.FirstOrDefault(d => d.ProductId == productId);

        if (details == null) return null;

        return new ProductDetailsDto
        {
            Id = details.Id,
            ProductId = details.ProductId,
            Brand = details.Brand,
            ProductionDate = details.ProductionDate, // اضافه شد
            ExpireDate = details.ExpireDate,
            BatchNumber = details.BatchNumber,         // اضافه شد
            ReturnConditions = details.ReturnConditions,
            WashingConditions = details.WashingConditions,
            Height = details.Height,
            Length = details.Length,
            Width = details.Width,
            Radius = details.Radius,
            Weight = details.Weight,
            Material = details.Material,
            CountryOfOrigin = details.CountryOfOrigin
        };
    }

    public async Task<bool> UpsertDetailsAsync(UpsertProductDetailsDto dto)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(dto.ProductId);
        if (product == null) return false;

        var detailsList = await _unitOfWork.Repository<ProductDetails>().GetAllAsync();
        var details = detailsList.FirstOrDefault(d => d.ProductId == dto.ProductId);

        if (details == null)
        {
            details = new ProductDetails
            {
                ProductId = dto.ProductId,
                Brand = dto.Brand,
                ProductionDate = dto.ProductionDate, // اضافه شد
                ExpireDate = dto.ExpireDate,
                BatchNumber = dto.BatchNumber,         // اضافه شد
                ReturnConditions = dto.ReturnConditions,
                WashingConditions = dto.WashingConditions,
                Height = dto.Height,
                Length = dto.Length,
                Width = dto.Width,
                Radius = dto.Radius,
                Weight = dto.Weight,
                Material = dto.Material,
                CountryOfOrigin = dto.CountryOfOrigin,
                IsActive = true
            };
            await _unitOfWork.Repository<ProductDetails>().AddAsync(details);
        }
        else
        {
            details.Brand = dto.Brand;
            details.ProductionDate = dto.ProductionDate; // اضافه شد
            details.ExpireDate = dto.ExpireDate;
            details.BatchNumber = dto.BatchNumber;         // اضافه شد
            details.ReturnConditions = dto.ReturnConditions;
            details.WashingConditions = dto.WashingConditions;
            details.Height = dto.Height;
            details.Length = dto.Length;
            details.Width = dto.Width;
            details.Radius = dto.Radius;
            details.Weight = dto.Weight;
            details.Material = dto.Material;
            details.CountryOfOrigin = dto.CountryOfOrigin;
            details.EditDateUtc = DateTime.UtcNow;

            _unitOfWork.Repository<ProductDetails>().Update(details);
        }

        return await _unitOfWork.CompleteAsync() > 0;
    }

    // **************** مدیریت رنگ‌ها و واریانت‌ها ****************

    public async Task<IEnumerable<ProductColorDto>> GetColorsByProductIdAsync(int productId)
    {
        var colorsList = await _unitOfWork.Repository<ProductColor>().GetAllAsync();
        var colors = colorsList.Where(c => c.ProductId == productId && c.IsActive);

        return colors.Select(c => new ProductColorDto
        {
            Id = c.Id,
            ProductId = c.ProductId,
            ColorName = c.ColorName,
            ColorCode = c.ColorCode
        });
    }

    public async Task<bool> UpsertColorAsync(UpsertProductColorDto dto)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(dto.ProductId);
        if (product == null) return false;

        var colorsList = await _unitOfWork.Repository<ProductColor>().GetAllAsync();
        var color = dto.Id > 0
            ? colorsList.FirstOrDefault(c => c.Id == dto.Id)
            : colorsList.FirstOrDefault(c => c.ProductId == dto.ProductId && c.ColorName == dto.ColorName);

        if (color == null)
        {
            color = new ProductColor
            {
                ProductId = dto.ProductId,
                ColorName = dto.ColorName,
                ColorCode = dto.ColorCode,
                IsActive = true
            };
            await _unitOfWork.Repository<ProductColor>().AddAsync(color);
        }
        else
        {
            color.ColorName = dto.ColorName;
            color.ColorCode = dto.ColorCode;
            color.EditDateUtc = DateTime.UtcNow;
            _unitOfWork.Repository<ProductColor>().Update(color);
        }

        return await _unitOfWork.CompleteAsync() > 0;
    }

    public async Task<bool> DeleteColorAsync(int colorId)
    {
        var color = await _unitOfWork.Repository<ProductColor>().GetByIdAsync(colorId);
        if (color == null) return false;

        color.IsActive = false;
        color.EditDateUtc = DateTime.UtcNow;
        _unitOfWork.Repository<ProductColor>().Update(color);

        return await _unitOfWork.CompleteAsync() > 0;
    }

    public async Task<IEnumerable<ProductVariantDto>> GetVariantsByProductIdAsync(int productId)
    {
        // از آنجا که واریانت به Color وصل است و Color به Product وصل است:
        var colors = await _unitOfWork.Repository<ProductColor>().GetAllAsync();
        var colorIds = colors.Where(c => c.ProductId == productId && c.IsActive).Select(c => c.Id).ToList();

        var variantsList = await _unitOfWork.Repository<ProductVariant>().GetAllAsync();
        var variants = variantsList.Where(v => v.ProductColorId.HasValue && colorIds.Contains(v.ProductColorId.Value) && v.IsActive);

        return variants.Select(v => new ProductVariantDto
        {
            Id = v.Id,
            ProductColorId = v.ProductColorId ?? 0,
            Price = v.PriceOverride ?? 0, // یا فیلد قیمت متناسب با ساختار
            StockQuantity = 0, // اگر از WarehouseInventory می‌آید یا فیلد جداست
            Sku = v.Sku
        });
    }

    public async Task<bool> UpsertVariantAsync(UpsertProductVariantDto dto)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(dto.ProductId);
        if (product == null) return false;

        var variant = dto.Id > 0
            ? await _unitOfWork.Repository<ProductVariant>().GetByIdAsync(dto.Id)
            : null;

        if (variant == null)
        {
            variant = new ProductVariant
            {
                ProductId = dto.ProductId,
                ProductColorId = dto.ProductColorId,
                Sku = dto.Sku,
                Barcode = dto.Barcode,
                VariantName = dto.VariantName,
                Size = dto.Size,
                PriceOverride = dto.PriceOverride,
                Weight = dto.Weight,
                IsDefault = dto.IsDefault,
                IsActive = true
            };
            await _unitOfWork.Repository<ProductVariant>().AddAsync(variant);
        }
        else
        {
            variant.ProductColorId = dto.ProductColorId;
            variant.Sku = dto.Sku;
            variant.Barcode = dto.Barcode;
            variant.VariantName = dto.VariantName;
            variant.Size = dto.Size;
            variant.PriceOverride = dto.PriceOverride;
            variant.Weight = dto.Weight;
            variant.IsDefault = dto.IsDefault;
            variant.EditDateUtc = DateTime.UtcNow;

            _unitOfWork.Repository<ProductVariant>().Update(variant);
        }

        return await _unitOfWork.CompleteAsync() > 0;
    }

    public async Task<bool> DeleteVariantAsync(int variantId)
    {
        var variant = await _unitOfWork.Repository<ProductVariant>().GetByIdAsync(variantId);
        if (variant == null) return false;

        variant.IsActive = false;
        variant.EditDateUtc = DateTime.UtcNow;
        _unitOfWork.Repository<ProductVariant>().Update(variant);

        return await _unitOfWork.CompleteAsync() > 0;
    }

    public async Task<IEnumerable<ProductVariantDto>> GetVariantsByColorIdAsync(int colorId)
    {
        var variantsList = await _unitOfWork.Repository<ProductVariant>().GetAllAsync();
        var variants = variantsList.Where(v => v.ProductColorId == colorId && v.IsActive);

        return variants.Select(v => new ProductVariantDto
        {
            Id = v.Id,
            ProductColorId = v.ProductColorId ?? 0,
            Price = v.PriceOverride ?? 0,
            StockQuantity = 0,
            Sku = v.Sku
        });
    }
}
