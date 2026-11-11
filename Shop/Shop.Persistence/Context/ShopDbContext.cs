using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Common;
using Shop.Domain.Entities;

namespace Shop.Persistence.Context;

public class ShopDbContext(DbContextOptions<ShopDbContext> options) : DbContext(options)
{
    // DbSetهای موجودیت‌ها
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<AddressDetails> AddressDetails => Set<AddressDetails>();
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<ArticleCategory> ArticleCategories => Set<ArticleCategory>();
    public DbSet<ArticleImage> ArticleImages => Set<ArticleImage>();
    public DbSet<ArticleTag> ArticleTags => Set<ArticleTag>();
    public DbSet<ArticleTagMap> ArticleTagMaps => Set<ArticleTagMap>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<Images> Images => Set<Images>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderStatusHistory> OrderStatusHistories => Set<OrderStatusHistory>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Person> People => Set<Person>();
    public DbSet<PersonType> PersonTypes => Set<PersonType>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductColor> ProductColors => Set<ProductColor>();
    public DbSet<ProductDetails> ProductDetails => Set<ProductDetails>();
    public DbSet<ProductRelation> ProductRelations => Set<ProductRelation>();
    public DbSet<ProductReview> ProductReviews => Set<ProductReview>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<Shipping> Shippings => Set<Shipping>();
    public DbSet<ShoppingBasket> ShoppingBaskets => Set<ShoppingBasket>();
    public DbSet<ShoppingBasketItem> ShoppingBasketItems => Set<ShoppingBasketItem>();
    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();
    public DbSet<SiteView> SiteViews => Set<SiteView>();
    public DbSet<StateCity> StateCities => Set<StateCity>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<WarehouseInventory> WarehouseInventories => Set<WarehouseInventory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ۱. حل ریشه‌ای ارورهای Cascade Paths در سراسر دیتابیس
        // تمام کلیدهای خارجی رفتاری امن پیدا می‌کنند و چرخه‌های حذفی متوقف می‌شوند
        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // ۲. تنظیم روابط خاص محصولات مرتبط (در صورت نیاز)
        modelBuilder.Entity<ProductRelation>(entity =>
        {
            entity.HasOne(x => x.Product)
                .WithMany(x => x.RelatedProducts)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.RelatedProduct)
                .WithMany()
                .HasForeignKey(x => x.RelatedProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ۳. تنظیم پیش‌فرض نوع اعشار برای مبالغ و قیمت‌ها
        foreach (var property in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,2)");
        }
        modelBuilder.Entity<AddressDetails>()
        .Property(x => x.Latitude)
        .HasColumnType("decimal(9,6)");

        modelBuilder.Entity<AddressDetails>()
                .Property(x => x.Longitude)
                .HasColumnType("decimal(9,6)");

        // ۴. اعمال فیلتر سراسری Soft Delete برای تمام موجودیت‌های دارای IsActive
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var isActiveProperty = entityType.FindProperty("IsActive");
            if (isActiveProperty != null && isActiveProperty.ClrType == typeof(bool))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var propertyAccess = Expression.Property(parameter, isActiveProperty.PropertyInfo!);
                var filter = Expression.Lambda(propertyAccess, parameter);

                entityType.SetQueryFilter(filter);
            }
        }
    }

    // ۵. تبدیل فیزیکی Delete به حذف نرم (Soft Delete)
    private void ConvertDeletesToSoftDeletes()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Deleted)
            {
                var isActiveProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "IsActive");
                if (isActiveProperty != null)
                {
                    entry.State = EntityState.Modified;
                    isActiveProperty.CurrentValue = false;
                }
            }
        }
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ConvertDeletesToSoftDeletes();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        ConvertDeletesToSoftDeletes();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
