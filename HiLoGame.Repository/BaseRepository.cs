using HiLoGame.Entities;
using HiLoGame.Repository.Storage;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace HiLoGame.Repository;
[ExcludeFromCodeCoverage]
public abstract class BaseRepository<T> where T : BaseEntity
{
    public readonly Context _context;

    public BaseRepository(Context context)
    {
        _context = context;
    }

    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<T> GetAsync(int id)
        => await _context.Set<T>().FindAsync(id);

    public async Task<List<T>> GetAllAsync()
        => await _context.Set<T>().ToListAsync();

    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
    }
}

