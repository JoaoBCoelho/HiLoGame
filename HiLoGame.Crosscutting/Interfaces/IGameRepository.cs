using HiLoGame.Model;
using MongoDB.Driver;

namespace HiLoGame.Crosscutting.Interfaces
{
    public interface IGameRepository
    {
        Task AddAsync(Game game);
        Task UpdateAsync(Game game);
        Task<Game> GetAsync(Guid id);
        Task<List<Game>> GetAllAsync(FilterDefinition<Game> filter);
    }
}
