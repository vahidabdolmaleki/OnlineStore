namespace Shop.Application.DTOs.Orders;

public class OrderDetailsDto
{
    public int Id { get; set; }
    public int? PersonId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string? TrackingCode { get; set; }
    public string? CourierName { get; set; }
    public DateTime OrderDateUtc { get; set; }
}
