using Microsoft.EntityFrameworkCore;

namespace AudioBooks.Infrastructure.Repositories;

public class Repository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    public Repository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<T> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
        return result;
    }
    public async Task<List<T>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await _context.Set<T>().ToListAsync(cancellationToken);
        return result;
    }
    public async Task AddAsync(
        T entity,
        CancellationToken cancellationToken = default)
    {
        await _context.Set<T>().AddAsync(entity, cancellationToken);
    }
    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
    }
    public async Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
    }
}
