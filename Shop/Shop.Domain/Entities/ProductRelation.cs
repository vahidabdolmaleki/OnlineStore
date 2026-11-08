using Shop.Domain.Common;
using Shop.Domain.Enums;

namespace Shop.Domain.Entities
{
    public class ProductRelation : BaseEntity
    {
        public int ProductId { get; set; }

        public int RelatedProductId { get; set; }

        public ProductRelationType RelationType { get; set; }

        public int SortOrder { get; set; }

        public Product Product { get; set; } = null!;

        public Product RelatedProduct { get; set; } = null!;
    }


}