using Shop.Domain.Common;

namespace Shop.Domain.Entities
{
    public class ShoppingBasket : BaseEntity
    {
        public int? PersonId { get; set; }

        public string? GuestToken { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAtUtc { get; set; }

        public Person? Person { get; set; }

        public ICollection<ShoppingBasketItem> Items { get; set; }
            = new List<ShoppingBasketItem>();
    }
}