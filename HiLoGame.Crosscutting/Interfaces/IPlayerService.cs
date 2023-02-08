using HiLoGame.DTO.Response;

namespace HiLoGame.Crosscutting.Interfaces
{
    public interface IPlayerService
    {
        Task<PlayerDTO> CreatePlayerAsync(string name);
        Task<PlayerDTO> GetAsync(int id);
        Task<List<PlayerDTO>> GetAllAsync();
    }
}
