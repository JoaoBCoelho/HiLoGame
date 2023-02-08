using AutoMapper;
using HiLoGame.Common.Exceptions;
using HiLoGame.Crosscutting.Interfaces;
using HiLoGame.DTO.Response;
using HiLoGame.Entities;

namespace HiLoGame.Service
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _repository;
        private readonly IMapper _mapper;

        public PlayerService(IMapper mapper, IPlayerRepository repository)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PlayerDTO> CreatePlayerAsync(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ValidationException("The player name must be informed.");

            var player = new Player(name);

            await _repository.AddAsync(player);

            return _mapper.Map<PlayerDTO>(player);
        }

        public async Task<List<PlayerDTO>> GetAllAsync()
        {
            var players = await _repository.GetAllAsync();

            return _mapper.Map<List<PlayerDTO>>(players);
        }

        public async Task<PlayerDTO> GetAsync(int id)
        {
            var player = await _repository.GetAsync(id);

            return _mapper.Map<PlayerDTO>(player);
        }
    }
}
