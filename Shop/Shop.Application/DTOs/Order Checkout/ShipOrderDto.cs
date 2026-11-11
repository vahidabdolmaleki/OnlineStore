namespace Shop.Application.DTOs.Orders;

public class ShipOrderDto
{
    public string TrackingCode { get; set; } = string.Empty;
    public string CourierName { get; set; } = string.Empty;
}
