using Shop.Domain.Common;

namespace Shop.Domain.Entities
{
    public class ProductVariant : BaseEntity
    {
        public int ProductId { get; set; }

        public int? ProductColorId { get; set; }

        public string Sku { get; set; } = string.Empty;

        public string? Barcode { get; set; }

        public string? VariantName { get; set; }

        public string? Size { get; set; }

        public decimal? PriceOverride { get; set; }

        public decimal? Weight { get; set; }

        public bool IsDefault { get; set; }

        public Product Product { get; set; } = null!;

        public ProductColor? ProductColor { get; set; }

        public ICollection<ShoppingBasketItem> ShoppingBasketItems { get; set; }
            = new List<ShoppingBasketItem>();

        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();

        public ICollection<WarehouseInventory> Inventories { get; set; }
            = new List<WarehouseInventory>();
    }
}