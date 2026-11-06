using Shop.Domain.Common;

namespace Shop.Domain.Entities
{
    public class ArticleTagMap : BaseEntity
    {
        public int ArticleId { get; set; }

        public int ArticleTagId { get; set; }

        public Article Article { get; set; } = null!;

        public ArticleTag ArticleTag { get; set; } = null!;
    }
}