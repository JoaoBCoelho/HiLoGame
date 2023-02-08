using HiLoGame.Entities;

namespace HiLoGame.Crosscutting.Interfaces
{
    public interface IGamePlayerInfoRepository
    {
        Task AddAsync(GamePlayerInfo playerInfo);
        Task UpdateAsync(GamePlayerInfo playerInfo);
        Task<GamePlayerInfo> GetAsync(int id);
        Task<List<GamePlayerInfo>> GetAllAsync();
    }
}
