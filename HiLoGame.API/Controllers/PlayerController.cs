using HiLoGame.Crosscutting.Interfaces;
using HiLoGame.DTO.Response;
using Microsoft.AspNetCore.Mvc;

namespace HiLoGameAPI.Controllers
{
    [ApiController]
    [Route("api/players")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(PlayerDTO))]
        public async Task<IActionResult> CreatePlayer([FromBody] string name)
        {
            var result = await _playerService.CreatePlayerAsync(name);

            return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(PlayerDTO))]
        public async Task<IActionResult> GetPlayer(int id)
        {
            var result = await _playerService.GetAsync(id);

            return result is not null
                ? Ok(result)
                : NotFound();
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<PlayerDTO>))]
        public async Task<IActionResult> GetPlayers()
        {
            var result = await _playerService.GetAllAsync();

            return result is not null && result.Any()
                ? Ok(result)
                : NoContent();
        }
    }
}
