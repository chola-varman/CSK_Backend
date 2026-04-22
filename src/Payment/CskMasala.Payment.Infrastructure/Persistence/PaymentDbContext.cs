using Microsoft.EntityFrameworkCore;

namespace CskMasala.Payment.Infrastructure.Persistence;

public class PaymentDbContext(DbContextOptions<PaymentDbContext> options) : DbContext(options)
{
    public DbSet<Domain.Payment> Payments => Set<Domain.Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Payment>(b =>
        {
            b.HasKey(p => p.Id);
            b.HasIndex(p => p.OrderId);
            b.Property(p => p.Amount).HasPrecision(18, 2);
            b.HasQueryFilter(p => !p.IsDeleted);
        });
    }
}
