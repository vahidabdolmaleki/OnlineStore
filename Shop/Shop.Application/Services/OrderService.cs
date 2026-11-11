using Shop.Application.DTOs;
using Shop.Application.DTOs.Orders;
using Shop.Application.Helpers;
using Shop.Application.Interfaces;
using Shop.Domain.Entities;
using Shop.Domain.Enums;
using Shop.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Application.Services;

public class OrderService(IUnitOfWork uow) : IOrderService
{

    public async Task<PagedOrderResultDto> GetOrdersAsync(OrderFilterDto filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        // اعتبارسنجی مقادیر صفحه‌بندی
        if (filter.PageNumber < 1) filter.PageNumber = 1;
        if (filter.PageSize is < 1 or > 100) filter.PageSize = 20;

        // تبدیل تاریخ‌های شمسی در صورت پر بودن
        DateTime? fromUtc = null;
        DateTime? toExclusiveUtc = null;

        if (!string.IsNullOrWhiteSpace(filter.FromDate))
        {
            fromUtc = PersianOrderDate.StartOfDayUtc(filter.FromDate);
        }

        if (!string.IsNullOrWhiteSpace(filter.ToDate))
        {
            toExclusiveUtc = PersianOrderDate.StartOfNextDayUtc(filter.ToDate);
        }

        if (fromUtc.HasValue && toExclusiveUtc.HasValue && fromUtc.Value >= toExclusiveUtc.Value)
        {
            throw new ArgumentException("تاریخ شروع نمی‌تواند بزرگتر یا مساوی تاریخ پایان باشد.");
        }

        var status = filter.Status;
        var paymentStatus = filter.PaymentStatus;
        var searchTerm = filter.SearchTerm?.Trim();

        // فیلتر پویا: هر شرطی که null یا خالی باشد، شرطش true شده و نادیده گرفته می‌شود
        var orders = await uow.Repository<Order>().GetAsync(o =>
            o.IsActive &&
            (!status.HasValue || o.Status == status.Value) &&
            (!paymentStatus.HasValue || o.PaymentStatus == paymentStatus.Value) &&
            (!fromUtc.HasValue || o.OrderDateUtc >= fromUtc.Value) &&
            (!toExclusiveUtc.HasValue || o.OrderDateUtc < toExclusiveUtc.Value) &&
            (string.IsNullOrWhiteSpace(searchTerm) ||
                o.OrderNumber.Contains(searchTerm) ||
                o.ReceiverName.Contains(searchTerm) ||
                o.ReceiverPhone.Contains(searchTerm) ||
                (o.TrackingCode != null && o.TrackingCode.Contains(searchTerm)))
        );

        var totalCount = orders.Count();

        var items = orders
            .OrderByDescending(o => o.OrderDateUtc)
            .ThenByDescending(o => o.Id)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(o => new OrderListDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerName = o.ReceiverName,
                Status = o.Status,
                PaymentStatus = o.PaymentStatus,
                FinalAmount = o.FinalAmount,
                OrderDateUtc = o.OrderDateUtc
            })
            .ToList();

        return new PagedOrderResultDto
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<int> CheckoutAsync(int personId, CheckoutDto checkoutDto)
    {
        if (personId <= 0)
            throw new ArgumentException("شناسه کاربر نامعتبر است.");

        ArgumentNullException.ThrowIfNull(checkoutDto);

        if (checkoutDto.AddressId <= 0)
            throw new ArgumentException("شناسه آدرس نامعتبر است.");

        // ---------- ۱. آدرس و جزئیات آن ----------
        var addresses = await uow.Repository<Address>()
            .GetAsync(x => x.Id == checkoutDto.AddressId && x.PersonId == personId);

        var address = addresses.FirstOrDefault();

        if (address is null)
            throw new InvalidOperationException("آدرس انتخاب‌شده پیدا نشد یا متعلق به شما نیست.");

        if (string.IsNullOrWhiteSpace(address.ReceiverName) ||
            string.IsNullOrWhiteSpace(address.ReceiverPhone))
            throw new InvalidOperationException("نام و شماره تماس گیرنده باید در آدرس ثبت شده باشد.");

        var addressDetailsList = await uow.Repository<AddressDetails>()
            .GetAsync(x => x.Id == address.AddressDetailsId);

        var addressDetails = addressDetailsList.FirstOrDefault();

        if (addressDetails is null)
            throw new InvalidOperationException("جزئیات آدرس انتخاب‌شده پیدا نشد.");

        // ---------- ۲. نام استان و شهر از StateCity ----------
        var cityList = await uow.Repository<StateCity>()
            .GetAsync(x => x.Id == addressDetails.StateCityId);

        var city = cityList.FirstOrDefault();

        if (city is null)
            throw new InvalidOperationException("شهر انتخاب‌شده در سیستم یافت نشد.");

        string stateTitle = string.Empty;

        if (city.ParentId.HasValue)
        {
            var stateList = await uow.Repository<StateCity>()
                .GetAsync(x => x.Id == city.ParentId.Value);

            stateTitle = stateList.FirstOrDefault()?.Title ?? string.Empty;
        }

        var shippingAddress = string.Join("، ", new List<string>
        {
            stateTitle,
            city.Title,
            addressDetails.Street,
            addressDetails.Alley,
            string.IsNullOrWhiteSpace(addressDetails.Number) ? null : $"پلاک {addressDetails.Number}",
            string.IsNullOrWhiteSpace(addressDetails.Unit) ? null : $"واحد {addressDetails.Unit}"
        }
        .Where(x => !string.IsNullOrWhiteSpace(x)));

        // ---------- ۳. سبد خرید و اقلام آن ----------
        var baskets = await uow.Repository<ShoppingBasket>()
            .GetAsync(x => x.PersonId == personId);

        var basket = baskets.OrderByDescending(x => x.UpdatedAtUtc).FirstOrDefault();

        if (basket is null)
            throw new InvalidOperationException("سبد خرید شما خالی است.");

        var basketItems = await uow.Repository<ShoppingBasketItem>()
            .GetAsync(x => x.ShoppingBasketId == basket.Id);

        if (basketItems.Count == 0)
            throw new InvalidOperationException("سبد خرید شما خالی است.");

        if (basketItems.Any(x =>
                x.Quantity <= 0 ||
                x.ProductId <= 0 ||
                x.ProductVariantId <= 0))
            throw new InvalidOperationException("یکی از اقلام سبد خرید اطلاعات نامعتبر دارد.");

        // ---------- ۴. دریافت اطلاعات قیمت روز محصولات و تنوع‌ها ----------
        var productIds = basketItems.Select(x => x.ProductId).Distinct().ToArray();
        var variantIds = basketItems.Select(x => x.ProductVariantId).Distinct().ToArray();

        var products = await uow.Repository<Product>()
            .GetAsync(x => productIds.Contains(x.Id));

        var variants = await uow.Repository<ProductVariant>()
            .GetAsync(x => variantIds.Contains(x.Id));

        var productsById = products.ToDictionary(x => x.Id);
        var variantsById = variants.ToDictionary(x => x.Id);

        // ---------- ۵. بررسی و کنترل موجودی در انبارها ----------
        var inventories = await uow.Repository<WarehouseInventory>()
            .GetAsync(x => productIds.Contains(x.ProductId) && variantIds.Contains(x.ProductVariantId));

        var inventoryMap = inventories
            .GroupBy(x => (x.ProductId, x.ProductVariantId))
            .ToDictionary(
                g => g.Key,
                g => g.ToList()
            );

        foreach (var item in basketItems)
        {
            if (!productsById.TryGetValue(item.ProductId, out var product))
                throw new InvalidOperationException($"محصول با شناسه {item.ProductId} پیدا نشد یا غیرفعال شده است.");

            if (!variantsById.TryGetValue(item.ProductVariantId, out var variant) || variant.ProductId != product.Id)
                throw new InvalidOperationException($"تنوع انتخاب‌شده برای محصول «{product.Name}» معتبر نیست.");

            var key = (item.ProductId, item.ProductVariantId);
            if (!inventoryMap.TryGetValue(key, out var itemInventories))
                throw new InvalidOperationException($"هیچ موجودی انباری برای محصول «{product.Name} - {variant.VariantName}» یافت نشد.");

            int totalAvailable = itemInventories.Sum(inv => inv.AvailableQuantity);
            if (totalAvailable < item.Quantity)
                throw new InvalidOperationException($"موجودی محصول «{product.Name} - {variant.VariantName}» کافی نیست. (موجودی: {totalAvailable}، درخواستی: {item.Quantity})");
        }

        // ---------- ۶. ساخت موجودیت سفارش ----------
        var order = new Order
        {
            OrderNumber = Guid.NewGuid().ToString("N").ToUpperInvariant(),
            PersonId = personId,
            AddressId = address.Id,
            Description = checkoutDto.Description,
            ReceiverName = address.ReceiverName!,
            ReceiverPhone = address.ReceiverPhone!,
            ShippingAddress = shippingAddress,
            PostalCode = addressDetails.PostalCode,
            DiscountAmount = 0,
            ShippingAmount = 0,
            TaxAmount = 0,
            IsActive = true
        };

        decimal subTotal = 0;

        // ---------- ۷. پردازش اقلام، کسر از انبار و افزودن به سفارش ----------
        foreach (var basketItem in basketItems)
        {
            var product = productsById[basketItem.ProductId];
            var variant = variantsById[basketItem.ProductVariantId];

            decimal currentPrice = product.Price;
            decimal lineTotal = currentPrice * basketItem.Quantity;
            subTotal += lineTotal;

            int neededQuantity = basketItem.Quantity;
            var itemInventories = inventoryMap[(basketItem.ProductId, basketItem.ProductVariantId)];

            foreach (var inv in itemInventories)
            {
                if (neededQuantity <= 0) break;

                int availableInThisWarehouse = inv.AvailableQuantity;
                if (availableInThisWarehouse <= 0) continue;

                int deduct = Math.Min(neededQuantity, availableInThisWarehouse);
                inv.Quantity -= deduct;
                neededQuantity -= deduct;

                uow.Repository<WarehouseInventory>().Update(inv);
            }

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductVariantId = variant.Id,
                ProductName = product.Name,
                ProductCode = product.Code,
                VariantName = variant.VariantName,
                Sku = variant.Sku,
                Quantity = basketItem.Quantity,
                UnitPrice = currentPrice,
                DiscountAmount = 0,
                TaxAmount = 0,
                TotalPrice = lineTotal,
                IsActive = true
            });
        }

        order.SubTotal = subTotal;
        order.FinalAmount = order.SubTotal
                          - order.DiscountAmount
                          + order.ShippingAmount
                          + order.TaxAmount;

        // ---------- ۸. ثبت در دیتابیس و پاک‌سازی سبد خرید ----------
        await uow.Repository<Order>().AddAsync(order);

        foreach (var basketItem in basketItems)
            uow.Repository<ShoppingBasketItem>().Delete(basketItem);

        await uow.CompleteAsync();

        return order.Id;
    }

    public async Task MarkPaidAsync(int orderId, PaymentMethod method, string? referenceId = null)
    {
        var order = await uow.Repository<Order>().GetByIdAsync(orderId)
            ?? throw new InvalidOperationException("سفارش یافت نشد.");

        if (order.Status != OrderStatus.Pending)
            throw new InvalidOperationException("وضعیت فعلی سفارش اجازه‌ی پرداخت را نمی‌دهد.");

        order.PaymentStatus = PaymentStatus.Paid;
        order.PaymentMethod = method;
        order.PaidAtUtc = DateTime.UtcNow;
        order.Status = OrderStatus.Confirmed;

        await uow.CompleteAsync();
    }

    public async Task MarkShippedAsync(int orderId, string trackingCode, string courierName)
    {
        var order = await uow.Repository<Order>().GetByIdAsync(orderId)
            ?? throw new InvalidOperationException("سفارش یافت نشد.");

        if (order.PaymentStatus != PaymentStatus.Paid)
            throw new InvalidOperationException("سفارش هنوز پرداخت نشده است.");

        order.Status = OrderStatus.Shipped;
        order.TrackingCode = trackingCode;
        order.CourierName = courierName;
        order.ShippedAtUtc = DateTime.UtcNow;

        await uow.CompleteAsync();
    }

    public async Task MarkDeliveredAsync(int orderId)
    {
        var order = await uow.Repository<Order>().GetByIdAsync(orderId)
            ?? throw new InvalidOperationException("سفارش یافت نشد.");

        order.Status = OrderStatus.Delivered;
        order.DeliveredAtUtc = DateTime.UtcNow;

        await uow.CompleteAsync();
    }

    public async Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
    {
        var order = await uow.Repository<Order>().GetByIdAsync(orderId)
            ?? throw new InvalidOperationException("سفارش یافت نشد.");

        order.Status = newStatus;
        await uow.CompleteAsync();
    }

    public async Task<OrderDetailsDto> GetOrderDetailsAsync(int orderId)
    {
        var order = await uow.Repository<Order>().GetByIdAsync(orderId)
            ?? throw new InvalidOperationException("سفارش یافت نشد.");

        return new OrderDetailsDto
        {
            Id = order.Id,
            PersonId = order.PersonId,
            Status = order.Status.ToString(),
            PaymentStatus = order.PaymentStatus.ToString(),
            TrackingCode = order.TrackingCode,
            CourierName = order.CourierName,
            OrderDateUtc = order.OrderDateUtc
        };
    }
    public async Task UpdateShippingInfoAsync(int orderId, UpdateShippingDto updateDto)
    {
        if (orderId <= 0)
            throw new ArgumentException("شناسه سفارش نامعتبر است.");

        ArgumentNullException.ThrowIfNull(updateDto);

        var order = await uow.Repository<Order>().GetByIdAsync(orderId)
            ?? throw new InvalidOperationException("سفارش مورد نظر یافت نشد.");

        // ویرایش یا اصلاح اطلاعات مرسوله
        order.TrackingCode = updateDto.TrackingCode;
        order.CourierName = updateDto.CourierName;

        // اگر سفارش قبلاً ارسال نشده بود و الان کد مرسوله ست شد، می‌توان وضعیت را هم به Shipped ارتقا داد (اختیاری)
        if (order.Status < OrderStatus.Shipped && !string.IsNullOrWhiteSpace(updateDto.TrackingCode))
        {
            order.Status = OrderStatus.Shipped;
            order.ShippedAtUtc ??= DateTime.UtcNow;
        }

        uow.Repository<Order>().Update(order);
        await uow.CompleteAsync();
    }
}
