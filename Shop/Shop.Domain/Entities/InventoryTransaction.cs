using Shop.Domain.Common;
using Shop.Domain.Enums;

namespace Shop.Domain.Entities
{
    public class InventoryTransaction : BaseEntity
    {
        public int WarehouseId { get; set; }

        public int ProductId { get; set; }

        public int ProductVariantId { get; set; }

        public InventoryTransactionType TransactionType { get; set; }

        public int Quantity { get; set; }

        public string? ReferenceType { get; set; }

        public int? ReferenceId { get; set; }

        public string? Description { get; set; }

        public Warehouse Warehouse { get; set; } = null!;

        public Product Product { get; set; } = null!;

        public ProductVariant ProductVariant { get; set; } = null!;
    }
}