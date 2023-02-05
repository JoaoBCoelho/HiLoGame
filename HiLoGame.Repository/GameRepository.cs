using HiLoGame.Crosscutting.Interfaces;
using HiLoGame.Model;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;

namespace HiLoGame.Repository
{
    [ExcludeFromCodeCoverage]
    public class GameRepository : IGameRepository
    {
        private readonly IMongoCollection<Game> _games;

        public GameRepository(IMongoDatabase database)
        {
            _games = database.GetCollection<Game>("Games");
        }

        public async Task AddAsync(Game game)
            => await _games.InsertOneAsync(game);

        public async Task<Game> GetAsync(Guid id)
            => await (await _games.FindAsync(i => i.Id == id)).FirstOrDefaultAsync();

        public async Task<List<Game>> GetAllAsync(FilterDefinition<Game> filter)
            => await (await _games.FindAsync(filter)).ToListAsync();

        public async Task UpdateAsync(Game game)
            => await _games.ReplaceOneAsync(g => g.Id == game.Id, game);
    }
}
