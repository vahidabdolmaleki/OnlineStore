using Shop.Domain.Enums;

namespace Shop.Application.DTOs.Orders;

public class OrderListDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public decimal FinalAmount { get; set; }
    public DateTime OrderDateUtc { get; set; }
}