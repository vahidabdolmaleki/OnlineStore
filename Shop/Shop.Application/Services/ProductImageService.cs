using Shop.Application.DTOs.Images;
using Shop.Application.DTOs.Products;
using Shop.Application.Interfaces;
using Shop.Domain.Entities;
using Shop.Domain.Interfaces;

namespace Shop.Application.Services;

public class ProductImageService(IUnitOfWork unitOfWork) : IProductImageService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

    public async Task<IEnumerable<ProductImageDto>> GetByProductIdAsync(int productId)
    {
        var images = await _unitOfWork.Repository<Images>()
            .GetAsync(x => x.ProductId == productId && x.IsActive);

        return images
            .OrderByDescending(x => x.IsMain)
            .ThenBy(x => x.SortOrder)
            .Select(MapToDto);
    }

    public async Task<IEnumerable<ProductImageDto>> GetByVariantIdAsync(int variantId)
    {
        var images = await _unitOfWork.Repository<Images>()
            .GetAsync(x => x.ProductVariantId == variantId && x.IsActive);

        return images
            .OrderByDescending(x => x.IsMain)
            .ThenBy(x => x.SortOrder)
            .Select(MapToDto);
    }

    public async Task<ProductImageDto?> GetByIdAsync(int imageId)
    {
        var image = await _unitOfWork.Repository<Images>()
            .GetByIdAsync(imageId);

        return image is { IsActive: true } ? MapToDto(image) : null;
    }

    public async Task<ProductImageDto> UploadImageAsync(
        UploadProductImageDto dto,
        string webRootPath)
    {
        if (dto.File is null || dto.File.Length == 0)
            throw new ArgumentException("فایل تصویر الزامی است.");

        if (!dto.ProductId.HasValue && !dto.ProductVariantId.HasValue)
            throw new ArgumentException("شناسه محصول یا واریانت الزامی است.");

        var extension = Path.GetExtension(dto.File.FileName);

        if (!AllowedExtensions.Contains(extension))
            throw new InvalidOperationException(
                "فرمت فایل معتبر نیست. فقط JPG، PNG و WEBP مجاز هستند.");

        var folder = Path.Combine(webRootPath, "uploads", "products");
        Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var physicalPath = Path.Combine(folder, fileName);

        await using (var stream = new FileStream(
            physicalPath,
            FileMode.CreateNew))
        {
            await dto.File.CopyToAsync(stream);
        }

        try
        {
            if (dto.IsMain && dto.ProductId.HasValue)
                await UnsetOtherMainImagesAsync(dto.ProductId.Value);

            var image = new Images
            {
                ProductId = dto.ProductId,
                ProductVariantId = dto.ProductVariantId,
                Url = $"/uploads/products/{fileName}",
                FileName = fileName,
                OriginalFileName = Path.GetFileName(dto.File.FileName),
                FileType = dto.File.ContentType,
                FileSize = dto.File.Length,
                AltText = dto.AltText,
                SortOrder = dto.SortOrder,
                IsMain = dto.IsMain
            };

            await _unitOfWork.Repository<Images>().AddAsync(image);
            await _unitOfWork.CompleteAsync();

            return MapToDto(image);
        }
        catch
        {
            // اگر ثبت در دیتابیس ناموفق شد، فایلِ تازه‌آپلودشده باقی نماند.
            if (File.Exists(physicalPath))
                File.Delete(physicalPath);

            throw;
        }
    }

    public async Task<bool> SetMainImageAsync(int imageId)
    {
        var repo = _unitOfWork.Repository<Images>();
        var target = await repo.GetByIdAsync(imageId);

        if (target is null || !target.IsActive || !target.ProductId.HasValue)
            return false;

        await UnsetOtherMainImagesAsync(
            target.ProductId.Value,
            exceptImageId: target.Id);

        target.IsMain = true;
        target.EditDateUtc = DateTime.UtcNow;
        repo.Update(target);

        await _unitOfWork.CompleteAsync();
        return true;
    }

    public async Task<bool> DeleteImageAsync(
        int imageId,
        string webRootPath)
    {
        var repo = _unitOfWork.Repository<Images>();
        var image = await repo.GetByIdAsync(imageId);

        if (image is null || !image.IsActive)
            return false;

        image.IsActive = false;
        image.EditDateUtc = DateTime.UtcNow;
        repo.Update(image);

        // اول حذف منطقی ثبت می‌شود؛ بعد فایل فیزیکی پاک می‌شود.
        await _unitOfWork.CompleteAsync();

        var physicalPath = Path.Combine(
            webRootPath,
            "uploads",
            "products",
            Path.GetFileName(image.FileName));

        if (File.Exists(physicalPath))
            File.Delete(physicalPath);

        return true;
    }

    private async Task UnsetOtherMainImagesAsync(
        int productId,
        int? exceptImageId = null)
    {
        var repo = _unitOfWork.Repository<Images>();

        var mainImages = await repo.GetAsync(x =>
            x.ProductId == productId &&
            x.IsActive &&
            x.IsMain &&
            (!exceptImageId.HasValue || x.Id != exceptImageId.Value));

        foreach (var item in mainImages)
        {
            // GetAsync خروجی AsNoTracking دارد؛ این نمونه قابل ویرایش است.
            var trackedImage = await repo.GetByIdAsync(item.Id);

            if (trackedImage is null || !trackedImage.IsActive)
                continue;

            trackedImage.IsMain = false;
            trackedImage.EditDateUtc = DateTime.UtcNow;
            repo.Update(trackedImage);
        }
    }

    private static ProductImageDto MapToDto(Images image) => new()
    {
        Id = image.Id,
        ProductId = image.ProductId,
        ProductVariantId = image.ProductVariantId,
        Url = image.Url,
        FileName = image.FileName,
        OriginalFileName = image.OriginalFileName,
        FileType = image.FileType,
        FileSize = image.FileSize,
        AltText = image.AltText,
        SortOrder = image.SortOrder,
        IsMain = image.IsMain
    };
}
