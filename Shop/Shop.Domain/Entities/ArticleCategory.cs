using Shop.Domain.Common;

namespace Shop.Domain.Entities
{
    public class ArticleCategory : BaseEntity
    {
        public string Title { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int? ParentId { get; set; }

        public string? MetaTitle { get; set; }

        public string? MetaDescription { get; set; }

        public int SortOrder { get; set; }

        public ArticleCategory? Parent { get; set; }

        public ICollection<ArticleCategory> Children { get; set; }
            = new List<ArticleCategory>();

        public ICollection<Article> Articles { get; set; }
            = new List<Article>();
    }
}