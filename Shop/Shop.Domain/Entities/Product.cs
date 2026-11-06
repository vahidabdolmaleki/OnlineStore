using Shop.Domain.Common;
using Shop.Domain.Entities;
using static System.Net.Mime.MediaTypeNames;

public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public decimal? DiscountPrice { get; set; }

        public int CategoryId { get; set; }

        public bool HasVariants { get; set; }

        public bool IsFeatured { get; set; }

        public int ViewCount { get; set; }

        public Category Category { get; set; } = null!;

        public ProductDetails? ProductDetails { get; set; }

        public ICollection<ProductColor> Colors { get; set; }
            = new List<ProductColor>();

        public ICollection<ProductVariant> Variants { get; set; }
            = new List<ProductVariant>();

        public ICollection<ProductRelation> RelatedProducts { get; set; }
            = new List<ProductRelation>();

        public ICollection<Images> Images { get; set; }
            = new List<Images>();

        public ICollection<ShoppingBasketItem> ShoppingBasketItems { get; set; }
            = new List<ShoppingBasketItem>();

        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();

        public ICollection<ProductReview> Reviews { get; set; }
            = new List<ProductReview>();

        public ICollection<WarehouseInventory> Inventories { get; set; }
            = new List<WarehouseInventory>();
    }
