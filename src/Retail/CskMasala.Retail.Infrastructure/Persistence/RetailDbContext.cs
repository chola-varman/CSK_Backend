using CskMasala.Retail.Domain;
using Microsoft.EntityFrameworkCore;

namespace CskMasala.Retail.Infrastructure.Persistence;

public class RetailDbContext(DbContextOptions<RetailDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<ShippingRate> ShippingRates => Set<ShippingRate>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(b =>
        {
            b.HasKey(c => c.Id);
            b.HasQueryFilter(c => !c.IsDeleted);
        });

        modelBuilder.Entity<Product>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Price).HasPrecision(18, 2);
            b.Property(p => p.ImageUrls).HasColumnType("text[]");
            b.HasQueryFilter(p => !p.IsDeleted);
        });

        modelBuilder.Entity<Coupon>(b =>
        {
            b.HasKey(c => c.Id);
            b.HasIndex(c => c.Code).IsUnique();
            b.Property(c => c.DiscountValue).HasPrecision(18, 2);
            b.Property(c => c.MinOrderAmount).HasPrecision(18, 2);
            b.Property(c => c.UsedCount).IsConcurrencyToken();
            b.HasQueryFilter(c => !c.IsDeleted);
        });

        modelBuilder.Entity<ShippingRate>(b =>
        {
            b.HasKey(s => s.Id);
            b.Property(s => s.Rate).HasPrecision(18, 2);
            b.HasQueryFilter(s => !s.IsDeleted);
        });

        modelBuilder.Entity<Order>(b =>
        {
            b.HasKey(o => o.Id);
            b.Property(o => o.TotalAmount).HasPrecision(18, 2);
            b.Property(o => o.DiscountAmount).HasPrecision(18, 2);
            b.Property(o => o.ShippingAmount).HasPrecision(18, 2);
            b.Property(o => o.FinalAmount).HasPrecision(18, 2);
            b.OwnsOne(o => o.ShippingAddress);
            b.HasMany(o => o.Items).WithOne().HasForeignKey(i => i.OrderId).OnDelete(DeleteBehavior.Cascade);
            b.HasQueryFilter(o => !o.IsDeleted);
        });

        modelBuilder.Entity<OrderItem>(b =>
        {
            b.HasKey(i => i.Id);
            b.Property(i => i.UnitPrice).HasPrecision(18, 2);
        });
    }
}
