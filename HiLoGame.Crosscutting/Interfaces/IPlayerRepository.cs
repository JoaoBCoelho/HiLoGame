using HiLoGame.Entities;

namespace HiLoGame.Crosscutting.Interfaces
{
    public interface IPlayerRepository
    {
        Task AddAsync(Player player);
        Task<Player> GetAsync(int id);
        Task<List<Player>> GetAllAsync();
        Task UpdateAsync(Player player);
    }
}
