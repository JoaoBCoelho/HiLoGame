using AutoMapper;
using HiLoGame.Crosscutting.Dtos;
using HiLoGame.Crosscutting.Dtos.Request;
using HiLoGame.Crosscutting.Dtos.Response;
using HiLoGame.Crosscutting.Exceptions;
using HiLoGame.Crosscutting.Interfaces;
using HiLoGame.Model;
using MongoDB.Driver;

namespace HiLoGame.Service
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _repository;
        private readonly IMapper _mapper;

        public GameService(IGameRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<NewGameResponseDTO> StartNewGameAsync(NewGameRequestDTO newGameDTO)
        {
            ValidateNewGameDTO(newGameDTO);

            var game = _mapper.Map<Game>(newGameDTO);

            await _repository.AddAsync(game);

            return _mapper.Map<NewGameResponseDTO>(game);
        }

        public async Task<NewGameResponseDTO> RestartGameAsync(Guid id)
        {
            var game = await _repository.GetAsync(id);

            ValidateGame(game, true);

            var newGame = _mapper.Map<Game>(game);

            await _repository.AddAsync(newGame);

            return _mapper.Map<NewGameResponseDTO>(game);
        }

        public async Task<GameGuessResponseDTO> TakeGameGuessesAsync(GameGuessRequestDTO gameGuessDTO)
        {
            var game = await _repository.GetAsync(gameGuessDTO.GameId);

            ValidateGame(game, false);
            ValidatePlayers(game, gameGuessDTO);

            var response = new GameGuessResponseDTO(game.Id);

            for (int i = 0; i < game.GameInstances.Count; i++)
            {
                var instance = game.GameInstances[i];
                var guess = gameGuessDTO.PlayerGuesses.FirstOrDefault(g => g.Id == instance.Id);
                var guessResult = TakeGuess(guess.Guess, instance.MisteryNumber);

                response.PlayerGuessesResponse.Add(new PlayerGuessResponseDTO
                {
                    Id = instance.Id,
                    Name = instance.PlayerName,
                    GuessResult = guessResult
                });

                if (guessResult == GuessResult.WON)
                {
                    instance.Winner = true;
                    game.Finished = true;
                    game.GameInstances[i] = instance;
                }
            }

            game.Attempts++;

            await _repository.UpdateAsync(game);

            return response;
        }

        private GuessResult TakeGuess(int guess, int misteryNumber)
        {
            if (guess > misteryNumber)
            {
                return GuessResult.LO;
            }
            else if (guess < misteryNumber)
            {
                return GuessResult.HI;
            }

            return GuessResult.WON;
        }

        public async Task<GameResponseDTO> GetGameAsync(Guid id)
            => _mapper.Map<GameResponseDTO>(await _repository.GetAsync(id));

        public async Task<List<GameResponseDTO>> GetGamesAsync(bool? finished)
        {
            var filter = new FilterDefinitionBuilder<Game>();
            FilterDefinition<Game> filterDefinition = null;

            if (finished.HasValue)
            {
                filterDefinition = filter.Eq(game => game.Finished, finished.Value);
            }

            var games = await _repository.GetAllAsync(filterDefinition);

            return _mapper.Map<List<GameResponseDTO>>(games);
        }

        private void ValidatePlayers(Game game, GameGuessRequestDTO gameGuessDTO)
        {
            if (game.GameInstances.Count > gameGuessDTO.PlayerGuesses.Count)
            {
                throw new BusinessException("Less guesses were reported than expected.");
            }

            if (game.GameInstances.Count < gameGuessDTO.PlayerGuesses.Count)
            {
                throw new BusinessException(
                    $"More guesses ({gameGuessDTO.PlayerGuesses.Count}) were informed than the expected ({game.GameInstances.Count}).");
            }

            if (!game.GameInstances
                .Select(s => s.Id)
                .All(gameGuessDTO.PlayerGuesses.Select(s => s.Id).Contains))
            {
                throw new BusinessException("Please, check if all Player Ids were informed correctly.");
            }

            if (gameGuessDTO.PlayerGuesses
                .Any(a=> a.Guess < game.MinValue || a.Guess > game.MaxValue))
            {
                throw new BusinessException($"All player guesses should be between {game.MinValue} and {game.MaxValue}");
            }
        }

        private static void ValidateGame(Game game, bool isRestart)
        {
            if (game is null)
            {
                throw new BusinessException("The GameId informed was not found.");
            }

            if (isRestart && !game.Finished)
            {
                throw new BusinessException($"The Game {game.Id} is not finished. You need to finish this game before restarting it.");
            }
            else if (!isRestart && game.Finished)
            {
                throw new BusinessException($"The Game {game.Id} is finished. Note: You can restart this game!");
            }
        }

        private static void ValidateNewGameDTO(NewGameRequestDTO newGameDTO)
        {
            if (!newGameDTO.Players.Any())
            {
                throw new BusinessException("At least one player should be informed to start a new game.");
            }

            if (newGameDTO.MinValue <= 0)
            {
                throw new BusinessException("The minimum value should be greater than zero.");
            }

            if (newGameDTO.MaxValue <= newGameDTO.MinValue)
            {
                throw new BusinessException("The maximum value should be greater than the minimum value.");
            }
        }
    }
}
