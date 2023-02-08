using HiLoGame.Crosscutting.Interfaces;
using HiLoGame.DTO.Request;
using HiLoGame.DTO.Response;
using Microsoft.AspNetCore.Mvc;

namespace HiLoGameAPI.Controllers
{
    [ApiController]
    [Route("api/games")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(NewGameResponseDTO))]
        public async Task<IActionResult> StartNewGame([FromBody] NewGameRequestDTO newGameRequest)
        {
            var result = await _gameService.StartNewGameAsync(newGameRequest);

            return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpPost("{id}/restart")]
        [ProducesResponseType(201, Type = typeof(NewGameResponseDTO))]
        public async Task<IActionResult> RestartGame(int id)
        {
            var result = await _gameService.RestartGameAsync(id);

            return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpPost("{id}/guess")]
        [ProducesResponseType(201, Type = typeof(GuessResponseDTO))]
        public async Task<IActionResult> TakeGuess(int id, [FromBody] GuessRequestDTO gameGuessRequest)
        {
            var result = await _gameService.TakeGuessAsync(id, gameGuessRequest);

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(GameResponseDTO))]
        public async Task<IActionResult> GetGame(int id)
        {
            var result = await _gameService.GetAsync(id);

            return result is not null
                ? Ok(result)
                : NotFound();
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<GameResponseDTO>))]
        public async Task<IActionResult> GetGames()
        {
            var result = await _gameService.GetAllAsync();

            return result is not null && result.Any()
                ? Ok(result)
                : NoContent();
        }
    }
}
