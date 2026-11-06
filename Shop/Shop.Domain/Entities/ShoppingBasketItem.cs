using Shop.Domain.Common;

namespace Shop.Domain.Entities
{
    public class ShoppingBasketItem : BaseEntity
    {
        public int ShoppingBasketId { get; set; }

        public int ProductId { get; set; }

        public int ProductVariantId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public ShoppingBasket ShoppingBasket { get; set; } = null!;

        public Product Product { get; set; } = null!;

        public ProductVariant ProductVariant { get; set; } = null!;
    }
}