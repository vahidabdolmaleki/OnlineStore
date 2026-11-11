using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.Orders;
using Shop.Application.Interfaces;
using Shop.Domain.Enums;
using System.Security.Claims;

namespace Shop.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] OrderFilterDto filter)
    {
        try
        {
            var result = await orderService.GetOrdersAsync(filter);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }



    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutDto checkoutDto)
    {
        //todo در مراحل بعدی احراز هویت با JWT، آیدی کاربر از توکن خوانده می‌شود
        // فعلاً برای تست، یک مقدار ثابت یا از هدر می‌گیریم
        // int personId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");


        try
        {
        // فعلاً برای تست سریع، فرض می‌کنیم کاربر ۱ است (بعداً با سیستم لاگین جایگزین کنید)
            int personId = 1;
            var orderId = await orderService.CheckoutAsync(personId, checkoutDto);
            return Ok(new { OrderId = orderId, Message = "سفارش با موفقیت ثبت شد." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "خطایی در ثبت سفارش رخ داد.", Details = ex.Message });
        }
    }
    [HttpPost("{orderId:int}/pay")]
    public async Task<IActionResult> Pay(int orderId, [FromBody] PayOrderDto payDto)
    {
        try
        {
            await orderService.MarkPaidAsync(orderId, payDto.Method, payDto.ReferenceId);
            return Ok(new { message = "پرداخت با موفقیت ثبت شد." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{orderId:int}/ship")]
    public async Task<IActionResult> Ship(int orderId, [FromBody] ShipOrderDto shipDto)
    {
        await orderService.MarkShippedAsync(orderId, shipDto.TrackingCode, shipDto.CourierName);
        return Ok(new { message = "کد مرسوله ثبت شد." });
    }

    [HttpPost("{orderId:int}/deliver")]
    public async Task<IActionResult> Deliver(int orderId)
    {
        await orderService.MarkDeliveredAsync(orderId);
        return Ok(new { message = "سفارش تحویل داده شد." });
    }

    [HttpGet("{orderId:int}/details")]
    public async Task<IActionResult> Details(int orderId)
    {
        var details = await orderService.GetOrderDetailsAsync(orderId);
        return Ok(details);
    }
    /// <summary>
    /// ویرایش یا تصحیح کد مرسوله پستی و شرکت حمل
    /// </summary>
    [HttpPut("{orderId:int}/shipping")]
    public async Task<IActionResult> UpdateShipping(int orderId, [FromBody] UpdateShippingDto dto)
    {
        try
        {
            await orderService.UpdateShippingInfoAsync(orderId, dto);
            return Ok(new { Message = "اطلاعات مرسوله با موفقیت به‌روزرسانی شد." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// تغییر آزادانه‌ی وضعیت سفارش (مثلاً بازگشت از تحویل، لغو سفارش، یا تأیید دستی)
    /// </summary>
    [HttpPut("{orderId:int}/status")]
    public async Task<IActionResult> ChangeOrderStatus(int orderId, [FromQuery] OrderStatus newStatus)
    {
        try
        {
            await orderService.UpdateOrderStatusAsync(orderId, newStatus);
            return Ok(new { Message = $"وضعیت سفارش با موفقیت به '{newStatus}' تغییر یافت." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }


}
