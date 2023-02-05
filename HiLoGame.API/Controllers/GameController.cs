using HiLoGame.Crosscutting.Dtos.Request;
using HiLoGame.Crosscutting.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HiLoGameAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost(Name = "StartNewGame")]
        public async Task<IActionResult> StartNewGame([FromBody] NewGameRequestDTO newGameRequest)
        {
            var result = await _gameService.StartNewGameAsync(newGameRequest);

            return Ok(result);
        }

        [HttpPost("game/{id}", Name = "RestartGame")]
        public async Task<IActionResult> RestartGame(Guid id)
        {
            var result = await _gameService.RestartGameAsync(id);

            return Ok(result);
        }

        [HttpPut(Name = "TakeGuesses")]
        public async Task<IActionResult> TakeGuesses([FromBody] GameGuessRequestDTO gameGuessRequest)
        {
            var result = await _gameService.TakeGameGuessesAsync(gameGuessRequest);

            return Ok(result);
        }

        [HttpGet("game/{id}", Name = "GetGame")]
        public async Task<IActionResult> GetGame(Guid id)
        {
            var result = await _gameService.GetGameAsync(id);

            return result is not null 
                ? Ok(result)
                : NoContent();
        }

        [HttpGet("games/{finished:bool?}", Name = "GetGames")]
        public async Task<IActionResult> GetGames(bool? finished = null)
        {
            var result = await _gameService.GetGamesAsync(finished);

            return result is not null
                ? Ok(result)
                : NoContent();
        }
    }
}
