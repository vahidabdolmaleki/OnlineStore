using Shop.Domain.Common;

namespace Shop.Domain.Entities
{
    public class ArticleImage : BaseEntity
    {
        public int ArticleId { get; set; }

        public string Url { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string? AltText { get; set; }

        public string? Caption { get; set; }

        public int SortOrder { get; set; }

        public Article Article { get; set; } = null!;
    }
}