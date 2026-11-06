using Shop.Domain.Common;
using Shop.Domain.Enums;

namespace Shop.Domain.Entities
{
    public class ProductReview : BaseEntity
    {
        public int ProductId { get; set; }

        public int PersonId { get; set; }

        public int Rating { get; set; }

        public string? Title { get; set; }

        public string? Comment { get; set; }

        public bool IsVerifiedPurchase { get; set; }

        public ReviewStatus Status { get; set; }
            = ReviewStatus.Pending;

        public Product Product { get; set; } = null!;

        public Person Person { get; set; } = null!;
    }
}