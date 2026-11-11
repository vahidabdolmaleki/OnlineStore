using Shop.Application.DTOs;
using Shop.Application.DTOs.Orders;
using Shop.Domain.Enums;

namespace Shop.Application.Interfaces;

public interface IOrderService
{

    // تبدیل سبد خرید جاری به سفارش
    Task<int> CheckoutAsync(int personId, CheckoutDto checkoutDto);
    Task<OrderDetailsDto> GetOrderDetailsAsync(int orderId);
    Task<PagedOrderResultDto> GetOrdersAsync(OrderFilterDto filter);

    // ✅ جدید
    Task MarkPaidAsync(int orderId, PaymentMethod method, string? referenceId = null);
    Task MarkShippedAsync(int orderId, string trackingCode, string courierName);
    Task MarkDeliveredAsync(int orderId);
    //
    //todo دریافت جزییات یک سفارش برای نمایش به کاربر
    // Task<OrderDto> GetOrderDetailsAsync(int orderId);
    // متدهای جدید برای مدیریت و ویرایش:
    Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);    
    Task UpdateShippingInfoAsync(int orderId, UpdateShippingDto updateDto);
}
