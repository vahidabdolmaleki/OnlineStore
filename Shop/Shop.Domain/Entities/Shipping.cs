using Shop.Domain.Common;
using Shop.Domain.Enums;

namespace Shop.Domain.Entities
{
    public class Shipping : BaseEntity
    {
        public int OrderId { get; set; }

        public string? CarrierName { get; set; }

        public string? TrackingNumber { get; set; }

        public ShippingStatus Status { get; set; }
            = ShippingStatus.Pending;

        public decimal Cost { get; set; }

        public DateTime? ShippedAtUtc { get; set; }

        public DateTime? DeliveredAtUtc { get; set; }

        public string? Description { get; set; }

        public Order Order { get; set; } = null!;
    }
}