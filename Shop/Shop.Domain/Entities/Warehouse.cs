using Shop.Domain.Common;

namespace Shop.Domain.Entities
{
    public class Warehouse : BaseEntity
    {
        public string Title { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int? AddressId { get; set; }

        public Address? Address { get; set; }

        public ICollection<WarehouseInventory> Inventories { get; set; }
            = new List<WarehouseInventory>();

        public ICollection<InventoryTransaction> Transactions { get; set; }
            = new List<InventoryTransaction>();
    }
}