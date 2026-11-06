using Shop.Domain.Common;
using Shop.Domain.Enums;
using System.Net;

namespace Shop.Domain.Entities
{
    public class WarehouseInventory : BaseEntity
    {
        public int WarehouseId { get; set; }

        public int ProductId { get; set; }

        public int ProductVariantId { get; set; }

        public int Quantity { get; set; }

        public int ReservedQuantity { get; set; }

        public int AvailableQuantity =>
            Quantity - ReservedQuantity;

        public Warehouse Warehouse { get; set; } = null!;

        public Product Product { get; set; } = null!;

        public ProductVariant ProductVariant { get; set; } = null!;
    }
}