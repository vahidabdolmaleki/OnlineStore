using Shop.Domain.Enums;

namespace Shop.Application.DTOs.Orders;

public class OrderFilterDto
{
    // فیلتر اختیاری وضعیت سفارش
    public OrderStatus? Status { get; set; }

    // فیلتر اختیاری وضعیت پرداخت
    public PaymentStatus? PaymentStatus { get; set; }

    // فیلتر اختیاری تاریخ شمسی (مثال: 1405/08/01)
    public string? FromDate { get; set; }

    // فیلتر اختیاری تاریخ شمسی (مثال: 1405/08/30)
    public string? ToDate { get; set; }

    // فیلتر اختیاری جستجوی متنی (شماره سفارش، نام گیرنده، شماره تماس، کد پیگیری)
    public string? SearchTerm { get; set; }

    // صفحه‌بندی
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
