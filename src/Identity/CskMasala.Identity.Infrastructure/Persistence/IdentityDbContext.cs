using CskMasala.Identity.Domain;
using Microsoft.EntityFrameworkCore;

namespace CskMasala.Identity.Infrastructure.Persistence;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(b =>
        {
            b.HasKey(u => u.Id);
            b.HasIndex(u => u.FirebaseUid).IsUnique();
            b.HasIndex(u => u.Email).IsUnique();
            b.OwnsOne(u => u.DefaultAddress);
            b.HasQueryFilter(u => !u.IsDeleted);
        });
    }
}
