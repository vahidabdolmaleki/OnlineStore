using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shop.Application.DTOs.ShoppingBasket;
using Shop.Application.Interfaces;
using Shop.Domain.Entities;
using Shop.Domain.Interfaces;

namespace Shop.Application.Services;

public class ShoppingBasketService(IUnitOfWork unitOfWork)
    : IShoppingBasketService
{
    public async Task<BasketDto?> GetBasketAsync(
        int? personId, string? guestToken)
    {
        ValidateOwner(personId, guestToken);

        var basket = await FindActiveBasketAsync(personId, guestToken);

        return basket is null
            ? null
            : await BuildBasketDtoAsync(basket);
    }

    public async Task<BasketDto> AddItemAsync(AddItemToBasketDto dto)
    {
        ValidateOwner(dto.PersonId, dto.GuestToken);

        if (dto.Quantity <= 0)
            throw new ArgumentException("تعداد باید بیشتر از صفر باشد.");

        var product = await unitOfWork.Repository<Product>()
            .GetByIdAsync(dto.ProductId);

        if (product is not { IsActive: true })
            throw new KeyNotFoundException("محصول فعال پیدا نشد.");

        var variant = await unitOfWork.Repository<ProductVariant>()
            .GetByIdAsync(dto.ProductVariantId);

        if (variant is not { IsActive: true } ||
            variant.ProductId != product.Id)
        {
            throw new KeyNotFoundException(
                "تنوع فعال متعلق به این محصول پیدا نشد.");
        }

        var basket = await FindActiveBasketAsync(
            dto.PersonId, dto.GuestToken);

        var existingItem = basket is null
            ? null
            : (await unitOfWork.Repository<ShoppingBasketItem>()
                .GetAsync(x =>
                    x.ShoppingBasketId == basket.Id &&
                    x.ProductId == dto.ProductId &&
                    x.ProductVariantId == dto.ProductVariantId &&
                    x.IsActive))
                .FirstOrDefault();

        var requestedQuantity = existingItem is null
            ? dto.Quantity
            : checked(existingItem.Quantity + dto.Quantity);

        var availableQuantity = await GetAvailableQuantityAsync(
            dto.ProductId, dto.ProductVariantId);

        if (availableQuantity < requestedQuantity)
            throw new ArgumentException(
                $"موجودی کافی نیست. موجودی قابل فروش: {availableQuantity}");

        // اولویت قیمت: قیمت اختصاصی تنوع، قیمت تخفیف‌خورده،
        // سپس قیمت عادی محصول.
        var unitPrice = variant.PriceOverride
            ?? product.DiscountPrice
            ?? product.Price;

        if (basket is null)
        {
            var now = DateTime.UtcNow;

            basket = new ShoppingBasket
            {
                PersonId = dto.PersonId,
                GuestToken = dto.PersonId.HasValue
                    ? null
                    : dto.GuestToken!.Trim(),
                CreatedAtUtc = now,
                UpdatedAtUtc = now,
                ExpiresAtUtc = now.AddDays(7)
            };

            await unitOfWork.Repository<ShoppingBasket>()
                .AddAsync(basket);

            // ابتدا شناسهٔ سبد ساخته می‌شود.
            await unitOfWork.CompleteAsync();
        }

        if (existingItem is null)
        {
            await unitOfWork.Repository<ShoppingBasketItem>()
                .AddAsync(new ShoppingBasketItem
                {
                    ShoppingBasketId = basket.Id,
                    ProductId = dto.ProductId,
                    ProductVariantId = dto.ProductVariantId,
                    Quantity = dto.Quantity,
                    UnitPrice = unitPrice
                });
        }
        else
        {
            var trackedItem = await unitOfWork
                .Repository<ShoppingBasketItem>()
                .GetByIdAsync(existingItem.Id);

            if (trackedItem is not { IsActive: true })
                throw new KeyNotFoundException("آیتم سبد پیدا نشد.");

            trackedItem.Quantity = requestedQuantity;
            trackedItem.UnitPrice = unitPrice;

            unitOfWork.Repository<ShoppingBasketItem>()
                .Update(trackedItem);
        }

        basket.UpdatedAtUtc = DateTime.UtcNow;
        unitOfWork.Repository<ShoppingBasket>().Update(basket);

        await unitOfWork.CompleteAsync();
        return await BuildBasketDtoAsync(basket);
    }

    public async Task<BasketDto> UpdateItemQuantityAsync(
        int itemId, UpdateBasketItemQuantityDto dto)
    {
        if (dto.Quantity <= 0)
            throw new ArgumentException("تعداد باید بیشتر از صفر باشد.");

        var item = await unitOfWork.Repository<ShoppingBasketItem>()
            .GetByIdAsync(itemId);

        if (item is not { IsActive: true })
            throw new KeyNotFoundException("آیتم سبد پیدا نشد.");

        var basket = await GetActiveBasketByIdAsync(
            item.ShoppingBasketId);

        var product = await unitOfWork.Repository<Product>()
            .GetByIdAsync(item.ProductId);

        var variant = await unitOfWork.Repository<ProductVariant>()
            .GetByIdAsync(item.ProductVariantId);

        if (product is not { IsActive: true } ||
            variant is not { IsActive: true } ||
            variant.ProductId != product.Id)
        {
            throw new KeyNotFoundException(
                "محصول یا تنوع فعال پیدا نشد.");
        }

        var availableQuantity = await GetAvailableQuantityAsync(
            item.ProductId, item.ProductVariantId);

        if (availableQuantity < dto.Quantity)
            throw new ArgumentException(
                $"موجودی کافی نیست. موجودی قابل فروش: {availableQuantity}");

        item.Quantity = dto.Quantity;
        item.UnitPrice = variant.PriceOverride
            ?? product.DiscountPrice
            ?? product.Price;

        basket.UpdatedAtUtc = DateTime.UtcNow;

        unitOfWork.Repository<ShoppingBasketItem>().Update(item);
        unitOfWork.Repository<ShoppingBasket>().Update(basket);

        await unitOfWork.CompleteAsync();
        return await BuildBasketDtoAsync(basket);
    }

    public async Task<BasketDto> RemoveItemAsync(int itemId)
    {
        var item = await unitOfWork.Repository<ShoppingBasketItem>()
            .GetByIdAsync(itemId);

        if (item is not { IsActive: true })
            throw new KeyNotFoundException("آیتم سبد پیدا نشد.");

        var basket = await GetActiveBasketByIdAsync(
            item.ShoppingBasketId);

        item.IsActive = false;
        basket.UpdatedAtUtc = DateTime.UtcNow;

        unitOfWork.Repository<ShoppingBasketItem>().Update(item);
        unitOfWork.Repository<ShoppingBasket>().Update(basket);

        await unitOfWork.CompleteAsync();
        return await BuildBasketDtoAsync(basket);
    }

    public async Task<bool> ClearBasketAsync(int basketId)
    {
        var basket = await unitOfWork.Repository<ShoppingBasket>()
            .GetByIdAsync(basketId);

        if (basket is not { IsActive: true })
            return false;

        var items = await unitOfWork.Repository<ShoppingBasketItem>()
            .GetAsync(x =>
                x.ShoppingBasketId == basketId && x.IsActive);

        foreach (var existingItem in items)
        {
            var item = await unitOfWork
                .Repository<ShoppingBasketItem>()
                .GetByIdAsync(existingItem.Id);

            if (item is not { IsActive: true })
                continue;

            item.IsActive = false;
            unitOfWork.Repository<ShoppingBasketItem>().Update(item);
        }

        basket.UpdatedAtUtc = DateTime.UtcNow;
        unitOfWork.Repository<ShoppingBasket>().Update(basket);

        await unitOfWork.CompleteAsync();
        return true;
    }

    private async Task<int> GetAvailableQuantityAsync(
        int productId, int variantId)
    {
        var inventories = await unitOfWork
            .Repository<WarehouseInventory>()
            .GetAsync(x =>
                x.ProductId == productId &&
                x.ProductVariantId == variantId &&
                x.IsActive);

        if (inventories.Count == 0)
            return 0;

        var warehouseIds = inventories
            .Select(x => x.WarehouseId)
            .Distinct()
            .ToArray();

        var activeWarehouses = await unitOfWork
            .Repository<Warehouse>()
            .GetAsync(x =>
                warehouseIds.Contains(x.Id) && x.IsActive);

        var activeWarehouseIds = activeWarehouses
            .Select(x => x.Id)
            .ToHashSet();

        // AvailableQuantity محاسباتی است؛ پس از واکشی
        // رکوردهای انبار، در حافظه جمع زده می‌شود.
        return inventories
            .Where(x => activeWarehouseIds.Contains(x.WarehouseId))
            .Sum(x => Math.Max(0, x.AvailableQuantity));
    }

    private async Task<ShoppingBasket?> FindActiveBasketAsync(
        int? personId, string? guestToken)
    {
        var now = DateTime.UtcNow;

        var baskets = personId.HasValue
            ? await unitOfWork.Repository<ShoppingBasket>()
                .GetAsync(x =>
                    x.PersonId == personId.Value &&
                    x.IsActive &&
                    x.ExpiresAtUtc > now)
            : await unitOfWork.Repository<ShoppingBasket>()
                .GetAsync(x =>
                    x.PersonId == null &&
                    x.GuestToken == guestToken!.Trim() &&
                    x.IsActive &&
                    x.ExpiresAtUtc > now);

        var found = baskets
            .OrderByDescending(x => x.UpdatedAtUtc)
            .FirstOrDefault();

        return found is null
            ? null
            : await unitOfWork.Repository<ShoppingBasket>()
                .GetByIdAsync(found.Id);
    }

    private async Task<ShoppingBasket> GetActiveBasketByIdAsync(
        int basketId)
    {
        var basket = await unitOfWork.Repository<ShoppingBasket>()
            .GetByIdAsync(basketId);

        if (basket is not { IsActive: true } ||
            basket.ExpiresAtUtc <= DateTime.UtcNow)
        {
            throw new KeyNotFoundException(
                "سبد خرید فعال پیدا نشد.");
        }

        return basket;
    }

    private async Task<BasketDto> BuildBasketDtoAsync(
        ShoppingBasket basket)
    {
        var items = await unitOfWork
            .Repository<ShoppingBasketItem>()
            .GetAsync(x =>
                x.ShoppingBasketId == basket.Id &&
                x.IsActive);

        var productIds = items
            .Select(x => x.ProductId)
            .Distinct()
            .ToArray();

        var products = productIds.Length == 0
            ? Array.Empty<Product>()
            : (await unitOfWork.Repository<Product>()
                .GetAsync(x => productIds.Contains(x.Id)))
                .ToArray();

        var productNames = products.ToDictionary(
            x => x.Id, x => x.Name);

        return new BasketDto
        {
            Id = basket.Id,
            PersonId = basket.PersonId,
            GuestToken = basket.GuestToken,
            CreatedAtUtc = basket.CreatedAtUtc,
            UpdatedAtUtc = basket.UpdatedAtUtc,
            ExpiresAtUtc = basket.ExpiresAtUtc,
            Items = items.Select(x => new BasketItemDto
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ProductName = productNames.TryGetValue(
                    x.ProductId, out var name)
                    ? name
                    : "محصول ناموجود",
                ProductVariantId = x.ProductVariantId,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice
            }).ToList()
        };
    }

    private static void ValidateOwner(
        int? personId, string? guestToken)
    {
        var hasPerson = personId.HasValue && personId.Value > 0;
        var hasGuest = !string.IsNullOrWhiteSpace(guestToken);

        if (hasPerson == hasGuest)
            throw new ArgumentException(
                "دقیقاً یکی از PersonId یا GuestToken باید ارسال شود.");
    }
}
