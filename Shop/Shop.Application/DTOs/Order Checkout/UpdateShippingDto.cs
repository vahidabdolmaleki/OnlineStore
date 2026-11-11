namespace Shop.Application.DTOs.Orders;

public class UpdateShippingDto
{
    public string TrackingCode { get; set; } = string.Empty;
    public string CourierName { get; set; } = string.Empty;
}