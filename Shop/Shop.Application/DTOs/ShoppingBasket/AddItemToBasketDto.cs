namespace Shop.Application.DTOs.ShoppingBasket;

public class AddItemToBasketDto
{
    public int? PersonId { get; set; }
    public string? GuestToken { get; set; }
    public int ProductId { get; set; }
    public int ProductVariantId { get; set; }
    public int Quantity { get; set; } = 1;
}
