using Shop.Domain.Common;
using Shop.Domain.Enums;

namespace Shop.Domain.Entities
{
    public class Order : BaseEntity
    {
        public string OrderNumber { get; set; } = string.Empty;

        public int? PersonId { get; set; }

        public int? AddressId { get; set; }

        public int? CouponId { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public PaymentStatus PaymentStatus { get; set; }
            = PaymentStatus.Unpaid;

        public string ReceiverName { get; set; } = string.Empty;

        public string ReceiverPhone { get; set; } = string.Empty;

        public string ShippingAddress { get; set; } = string.Empty;

        public string? PostalCode { get; set; }

        public decimal SubTotal { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal ShippingAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal FinalAmount { get; set; }

        public string? Description { get; set; }

        public string? TrackingCode { get; set; }

        public DateTime? PaidAtUtc { get; set; }

        public DateTime? CompletedAtUtc { get; set; }

        public Person? Person { get; set; }

        public Address? Address { get; set; }

        public Coupon? Coupon { get; set; }

        public ICollection<OrderItem> Items { get; set; }
            = new List<OrderItem>();

        public ICollection<Payment> Payments { get; set; }
            = new List<Payment>();

        public ICollection<OrderStatusHistory> StatusHistories { get; set; }
            = new List<OrderStatusHistory>();

        public ICollection<Shipping> Shippings { get; set; }
            = new List<Shipping>();

 

        // ✅ روش پرداخت (آنلاین، در محل، کارت به کارت، کیف پول)
        public PaymentMethod? PaymentMethod { get; set; }



        // ✅ نام شرکت حمل (پست، تیپاکس، ...)
        public string? CourierName { get; set; }

        // ✅ تاریخ‌های مهم چرخه

        public DateTime? ShippedAtUtc { get; set; }
        public DateTime? DeliveredAtUtc { get; set; }
        public DateTime? CancelledAtUtc { get; set; }

        // ✅ تاریخ/ساعت ثبت سفارش
        public DateTime OrderDateUtc { get; set; } = DateTime.UtcNow;
    }
}