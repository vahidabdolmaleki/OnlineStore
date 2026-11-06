using Shop.Domain.Common;

namespace Shop.Domain.Entities
{
    public class ProductColor : BaseEntity
    {
        public int ProductId { get; set; }

        public string ColorName { get; set; } = string.Empty;

        public string ColorCode { get; set; } = string.Empty;

        public Product Product { get; set; } = null!;

        public ICollection<ProductVariant> Variants { get; set; }
            = new List<ProductVariant>();
    }
}