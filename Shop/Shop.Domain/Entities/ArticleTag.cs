using Shop.Domain.Common;

namespace Shop.Domain.Entities
{
    public class ArticleTag : BaseEntity
    {
        public string Title { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public ICollection<ArticleTagMap> Articles { get; set; }
            = new List<ArticleTagMap>();
    }
}