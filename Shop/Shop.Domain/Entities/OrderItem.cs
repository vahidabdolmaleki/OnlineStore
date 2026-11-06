using Shop.Domain.Common;

namespace Shop.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }

        public int? ProductId { get; set; }

        public int? ProductVariantId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string ProductCode { get; set; } = string.Empty;

        public string? VariantName { get; set; }

        public string? Sku { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal TotalPrice { get; set; }

        public Order Order { get; set; } = null!;

        public Product? Product { get; set; }

        public ProductVariant? ProductVariant { get; set; }
    }
}