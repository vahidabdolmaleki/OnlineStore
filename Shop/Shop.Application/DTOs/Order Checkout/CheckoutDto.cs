namespace Shop.Application.DTOs.Orders;

public class CheckoutDto
{
    public int AddressId { get; set; }
    public string? Description { get; set; }
    //todo در آینده متد پرداخت یا کد تخفیف هم اینجا اضافه می‌شود
}
