using Shop.Domain.Common;
using Shop.Domain.Enums;

namespace Shop.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public int OrderId { get; set; }

        public decimal Amount { get; set; }

        public PaymentMethod Method { get; set; }

        public PaymentStatus Status { get; set; }
            = PaymentStatus.Unpaid;

        public string? GatewayName { get; set; }

        public string? Authority { get; set; }

        public string? TransactionId { get; set; }

        public string? ReferenceNumber { get; set; }

        public string? GatewayResponse { get; set; }

        public DateTime? PaidAtUtc { get; set; }

        public string? Description { get; set; }

        public Order Order { get; set; } = null!;
    }
}