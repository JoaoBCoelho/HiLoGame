using HiLoGame.Crosscutting.Dtos.Request;
using HiLoGame.Crosscutting.Dtos.Response;

namespace HiLoGame.Crosscutting.Interfaces
{
    public interface IGameService
    {
        Task<NewGameResponseDTO> StartNewGameAsync(NewGameRequestDTO newGameDTO);
        Task<GameResponseDTO> GetGameAsync(Guid id);
        Task<List<GameResponseDTO>> GetGamesAsync(bool? finished);
        Task<GameGuessResponseDTO> TakeGameGuessesAsync(GameGuessRequestDTO gameGuessRequest);
        Task<NewGameResponseDTO> RestartGameAsync(Guid id);
    }
}
