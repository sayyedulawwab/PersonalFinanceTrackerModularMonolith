using Common.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Modules.Users.Infrastructure.Repositories;

internal abstract class Repository<TEntity>(ApplicationDbContext dbContext)
    where TEntity : Entity<Guid>
{
    protected readonly ApplicationDbContext DbContext = dbContext;

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<TEntity>().ToListAsync(cancellationToken);
    }

    public virtual async Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<TEntity>()
            .FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public void Add(TEntity entity)
    {
        DbContext.Add(entity);
    }

    public void Remove(TEntity entity)
    {
        DbContext.Remove(entity);
    }

    public void Update(TEntity entity)
    {
        DbContext.Update(entity);
    }
}
