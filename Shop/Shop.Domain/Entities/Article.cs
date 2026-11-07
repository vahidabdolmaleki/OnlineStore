using Shop.Domain.Common;
using Shop.Domain.Enums;

namespace Shop.Domain.Entities
{
    public class Article : BaseEntity
    {
        // اطلاعات اصلی مقاله

        public string Title { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string? Summary { get; set; }

        public string Content { get; set; } = string.Empty;

        // تصویر شاخص

        public int? FeaturedImageId { get; set; }

        // دسته بندی

        public int? ArticleCategoryId { get; set; }

        // نویسنده

        public int? AuthorPersonId { get; set; }

        // اطلاعات سئو

        public string? MetaTitle { get; set; }

        public string? MetaDescription { get; set; }

        public string? CanonicalUrl { get; set; }

        public string? FocusKeyword { get; set; }

        public string? MetaKeywords { get; set; }

        // وضعیت انتشار

        public ArticleStatus Status { get; set; }
            = ArticleStatus.Draft;

        public DateTime? PublishedAtUtc { get; set; }

        public DateTime? ScheduledAtUtc { get; set; }

        // آمار

        public int ViewCount { get; set; } = 0;

        public int ReadingTimeMinutes { get; set; }

        public bool AllowComments { get; set; } = true;

        public bool IsFeatured { get; set; } = false;

        // ارتباطات

        public Images? FeaturedImage { get; set; }

        public ArticleCategory? ArticleCategory { get; set; }

        public Person? Author { get; set; }

        public ICollection<ArticleImage> Images { get; set; }
            = new List<ArticleImage>();

        public ICollection<ArticleTagMap> Tags { get; set; }
            = new List<ArticleTagMap>() ;
    }
}
