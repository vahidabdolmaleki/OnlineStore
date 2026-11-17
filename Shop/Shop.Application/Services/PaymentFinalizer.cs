using System.Data;
using Shop.Application.Interfaces;
using Shop.Domain.Entities;
using Shop.Domain.Enums;
using Shop.Domain.Interfaces;

namespace Shop.Application.Services;

public sealed class PaymentFinalizer(
    IUnitOfWork unitOfWork) : IPaymentFinalizer
{
    public async Task FinalizeAsync(
        int orderId,
        PaymentMethod method,
        string referenceNumber,
        CancellationToken cancellationToken = default)
    {
        if (orderId <= 0)
            throw new ArgumentException("شناسه سفارش نامعتبر است.");

        if (!Enum.IsDefined(typeof(PaymentMethod), method))
            throw new ArgumentException("روش پرداخت نامعتبر است.");

        if (string.IsNullOrWhiteSpace(referenceNumber))
            throw new ArgumentException("شماره مرجع پرداخت الزامی است.");

        referenceNumber = referenceNumber.Trim();

        await unitOfWork.ExecuteInTransactionAsync(
            async token =>
            {
                token.ThrowIfCancellationRequested();

                var orderRepository = unitOfWork.Repository<Order>();
                var paymentRepository = unitOfWork.Repository<Payment>();
                var inventoryRepository =
                    unitOfWork.Repository<WarehouseInventory>();

                // این متد در ریپازیتوری فعلی Tracking دارد.
                var order = await orderRepository.GetFirstOrDefaultAsync(
                    x => x.Id == orderId && x.IsActive,
                    x => x.Items);

                if (order is null)
                {
                    throw new InvalidOperationException(
                        "سفارش یافت نشد.");
                }

                // تکرار همان درخواست نباید دوباره موجودی را کم کند.
                if (order.PaymentStatus == PaymentStatus.Paid)
                {
                    var samePayment =
                        await paymentRepository.GetFirstOrDefaultAsync(
                            x => x.OrderId == orderId &&
                                 x.Status == PaymentStatus.Paid &&
                                 x.Method == method &&
                                 x.ReferenceNumber == referenceNumber);

                    if (samePayment is null)
                    {
                        throw new InvalidOperationException(
                            "سفارش قبلاً با پرداخت دیگری تسویه شده است.");
                    }

                    return;
                }

                if (order.Status != OrderStatus.Pending ||
                    order.PaymentStatus is not (
                        PaymentStatus.Unpaid or
                        PaymentStatus.Pending or
                        PaymentStatus.Failed))
                {
                    throw new InvalidOperationException(
                        "وضعیت سفارش اجازه نهایی‌سازی پرداخت را نمی‌دهد.");
                }

                if (order.FinalAmount <= 0)
                {
                    throw new InvalidOperationException(
                        "مبلغ سفارش برای این مسیر پرداخت باید مثبت باشد.");
                }

                if (order.Items.Count == 0 ||
                    order.Items.Any(x => x.Quantity <= 0))
                {
                    throw new InvalidOperationException(
                        "اقلام سفارش معتبر نیستند.");
                }

                var requestedItems = order.Items
                    .GroupBy(x => new
                    {
                        x.ProductId,
                        x.ProductVariantId
                    })
                    .Select(g => new
                    {
                        g.Key.ProductId,
                        g.Key.ProductVariantId,
                        Quantity = g.Sum(x => x.Quantity)
                    })
                    .OrderBy(x => x.ProductId)
                    .ThenBy(x => x.ProductVariantId)
                    .ToList();

                foreach (var item in requestedItems)
                {
                    token.ThrowIfCancellationRequested();

                    // GetAsync در ریپازیتوری فعلی AsNoTracking است.
                    var inventoryRows =
                        await inventoryRepository.GetAsync(
                            x => x.ProductId == item.ProductId &&
                                 x.ProductVariantId ==
                                 item.ProductVariantId);

                    // نمونه‌های Tracking را می‌گیریم تا تغییرات
                    // بدون Attach کردن نمونهٔ تکراری ذخیره شوند.
                    var inventories = new List<WarehouseInventory>();

                    foreach (var row in inventoryRows.OrderBy(x => x.Id))
                    {
                        token.ThrowIfCancellationRequested();

                        var inventory =
                            await inventoryRepository.GetByIdAsync(row.Id);

                        if (inventory is null || !inventory.IsActive)
                        {
                            throw new InvalidOperationException(
                                "اطلاعات موجودی کالا تغییر کرده است.");
                        }

                        inventories.Add(inventory);
                    }

                    var totalAvailable = inventories.Sum(
                        x => Math.Max(0, x.AvailableQuantity));

                    if (totalAvailable < item.Quantity)
                    {
                        throw new InvalidOperationException(
                            $"موجودی محصول {item.ProductId} و تنوع " +
                            $"{item.ProductVariantId} کافی نیست.");
                    }

                    var remaining = item.Quantity;

                    foreach (var inventory in inventories)
                    {
                        if (remaining == 0)
                            break;

                        var available = Math.Max(
                            0, inventory.AvailableQuantity);

                        var deduct = Math.Min(remaining, available);

                        inventory.Quantity -= deduct;
                        remaining -= deduct;
                    }
                }

                var now = DateTime.UtcNow;

                await paymentRepository.AddAsync(new Payment
                {
                    OrderId = order.Id,

                    // واحد مبلغ سیستم: ریال
                    Amount = order.FinalAmount,

                    Method = method,
                    Status = PaymentStatus.Paid,
                    ReferenceNumber = referenceNumber,
                    PaidAtUtc = now,
                    IsActive = true
                });

                order.PaymentStatus = PaymentStatus.Paid;
                order.PaymentMethod = method;
                order.PaidAtUtc = now;
                order.Status = OrderStatus.Confirmed;

                // ثبت مستقیم تاریخچه؛ نیازی به بارگذاری
                // مجموعهٔ StatusHistories سفارش نیست.
                await unitOfWork.Repository<OrderStatusHistory>()
                    .AddAsync(new OrderStatusHistory
                    {
                        OrderId = order.Id,
                        Status = OrderStatus.Confirmed,
                        Description =
                            "پرداخت تأیید و موجودی سفارش کسر شد.",
                        IsActive = true
                    });

                token.ThrowIfCancellationRequested();

                // سفارش و موجودی‌ها Tracking هستند.
                // بنابراین فراخوانی Update لازم نیست.
                await unitOfWork.CompleteAsync();
            },
            isolationLevel: IsolationLevel.Serializable,
            cancellationToken: cancellationToken);
    }
}
