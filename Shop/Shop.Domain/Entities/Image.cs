using Shop.Domain.Common;

namespace Shop.Domain.Entities
{
    public class Images : BaseEntity
    {
        public int? ProductId { get; set; }

        public int? ProductVariantId { get; set; }

        public int? PersonId { get; set; }

        public string Url { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string? OriginalFileName { get; set; }

        public string FileType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public string? AltText { get; set; }

        public int SortOrder { get; set; }

        public bool IsMain { get; set; }

        public Product? Product { get; set; }

        public ProductVariant? ProductVariant { get; set; }

        public Person? Person { get; set; }
    }
}