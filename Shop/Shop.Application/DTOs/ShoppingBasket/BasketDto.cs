namespace Shop.Application.DTOs.ShoppingBasket;

public class BasketDto
{
    public int Id { get; set; }
    public int? PersonId { get; set; }
    public string? GuestToken { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public List<BasketItemDto> Items { get; set; } = new();
    public decimal TotalAmount => Items.Sum(x => x.TotalPrice);
    public int TotalItemsCount => Items.Sum(x => x.Quantity);
}
