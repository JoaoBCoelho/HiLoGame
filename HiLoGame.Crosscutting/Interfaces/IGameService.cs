using HiLoGame.DTO.Request;
using HiLoGame.DTO.Response;

namespace HiLoGame.Crosscutting.Interfaces
{
    public interface IGameService
    {
        Task<NewGameResponseDTO> StartNewGameAsync(NewGameRequestDTO newGameDTO);
        Task<GameResponseDTO> GetAsync(int id);
        Task<List<GameResponseDTO>> GetAllAsync();
        Task<GuessResponseDTO> TakeGuessAsync(int gameId, GuessRequestDTO guessRequestDTO);
        Task<NewGameResponseDTO> RestartGameAsync(int id);
    }
}
