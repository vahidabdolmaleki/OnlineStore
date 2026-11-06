using Shop.Domain.Common;

public class Category : BaseEntity
    {
        public string Title { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int? ParentId { get; set; }

        public int SortOrder { get; set; }

        public string? MetaTitle { get; set; }

        public string? MetaDescription { get; set; }

        public Category? Parent { get; set; }

        public ICollection<Category> Children { get; set; }
            = new List<Category>();

        public ICollection<Product> Products { get; set; }
            = new List<Product>();
    }
