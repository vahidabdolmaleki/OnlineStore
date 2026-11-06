using Shop.Domain.Common;
using Shop.Domain.Enums;

namespace Shop.Domain.Entities
{
    public class Coupon : BaseEntity
    {
        public string Code { get; set; } = string.Empty;

        public string? Title { get; set; }

        public CouponType Type { get; set; }

        public decimal Value { get; set; }

        public decimal? MaxDiscountAmount { get; set; }

        public decimal? MinOrderAmount { get; set; }

        public int? UsageLimit { get; set; }

        public int UsageCount { get; set; }

        public int? UsageLimitPerPerson { get; set; }

        public DateTime StartDateUtc { get; set; }

        public DateTime EndDateUtc { get; set; }

        public bool IsPublic { get; set; }

        public ICollection<Order> Orders { get; set; }
            = new List<Order>();
    }
}