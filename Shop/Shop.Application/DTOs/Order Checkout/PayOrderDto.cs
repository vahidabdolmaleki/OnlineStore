using Shop.Domain.Enums;

namespace Shop.Application.DTOs.Orders;

public class PayOrderDto
{
    public PaymentMethod Method { get; set; }
    public string? ReferenceId { get; set; }   // شماره تراکنش یا رفرنس درگاه
}
