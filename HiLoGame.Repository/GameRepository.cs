using HiLoGame.Crosscutting.Interfaces;
using HiLoGame.Entities;
using HiLoGame.Repository.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Diagnostics.CodeAnalysis;

namespace HiLoGame.Repository;
[ExcludeFromCodeCoverage]
public class GameRepository : BaseRepository<Game>, IGameRepository
{
    public GameRepository(Context context) : base(context) { }

    public async Task<Game> GetAsync(int id)
        => await CreateIncludableQuery().FirstOrDefaultAsync(f => f.Id == id);

    public async Task<List<Game>> GetAllAsync()
        => await CreateIncludableQuery().ToListAsync();

    private IIncludableQueryable<Game, Player> CreateIncludableQuery()
    {
        return _context.Games
                        .Include(i => i.GamePlayerInfos)
                        .ThenInclude(i => i.Player);
    }
}