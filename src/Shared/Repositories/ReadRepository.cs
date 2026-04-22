using System.Linq.Expressions;
using CskMasala.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace CskMasala.Shared.Repositories;

public class ReadRepository<T>(DbContext db) : IReadRepository<T> where T : BaseEntity
{
    private readonly DbSet<T> _set = db.Set<T>();

    public Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _set.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, ct);

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default) =>
        await _set.AsNoTracking().Where(e => !e.IsDeleted).ToListAsync(ct);

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
        await _set.AsNoTracking().Where(predicate).Where(e => !e.IsDeleted).ToListAsync(ct);

    public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
        _set.AsNoTracking().AnyAsync(predicate, ct);
}
