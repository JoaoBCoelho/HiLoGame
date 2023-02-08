using HiLoGame.Entities;

namespace HiLoGame.Crosscutting.Interfaces
{
    public interface IGameRepository
    {
        Task AddAsync(Game game);
        Task UpdateAsync(Game game);
        Task<Game> GetAsync(int id);
        Task<List<Game>> GetAllAsync();
    }
}
