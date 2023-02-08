using AutoMapper;
using HiLoGame.Common;
using HiLoGame.Common.Exceptions;
using HiLoGame.Crosscutting.Interfaces;
using HiLoGame.DTO.Request;
using HiLoGame.DTO.Response;
using HiLoGame.Entities;

namespace HiLoGame.Service
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _repository;
        private readonly IGamePlayerInfoRepository _gamePlayerInfoRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IMapper _mapper;

        public GameService(
            IGameRepository repository,
            IGamePlayerInfoRepository gamePlayerInfoRepository,
            IPlayerRepository playerRepository,
            IMapper mapper)
        {
            _repository = repository;
            _gamePlayerInfoRepository = gamePlayerInfoRepository;
            _playerRepository = playerRepository;
            _mapper = mapper;
        }

        public async Task<NewGameResponseDTO> StartNewGameAsync(NewGameRequestDTO newGameDTO)
        {
            ValidateNewGameDTO(newGameDTO);
            await ValidateNewGamePlayersDTOAsync(newGameDTO.Players);

            var game = _mapper.Map<Game>(newGameDTO);

            await _repository.AddAsync(game);
            await UpdatePlayerGames(game);

            return _mapper.Map<NewGameResponseDTO>(game);
        }

        public async Task<NewGameResponseDTO> RestartGameAsync(int id)
        {
            var game = await _repository.GetAsync(id);

            ValidateGame(game, true);

            var newGame = _mapper.Map<Game>(game);

            await _repository.AddAsync(newGame);
            await UpdatePlayerGames(newGame);

            return _mapper.Map<NewGameResponseDTO>(newGame);
        }

        public async Task<GuessResponseDTO> TakeGuessAsync(int gameId, GuessRequestDTO guessRequestDTO)
        {
            var game = await _repository.GetAsync(gameId);

            ValidateGame(game, false);
            
            var player = await _playerRepository.GetAsync(guessRequestDTO.PlayerId);
            var playerInfo = game.GamePlayerInfos.FirstOrDefault(f => f.PlayerId == guessRequestDTO.PlayerId);
            
            ValidateGamePlayer(game, playerInfo, player);
            ValidateGuess(game, guessRequestDTO);

            var response = new GuessResponseDTO()
            {
                GameId = game.Id,
                PlayerId = player.Id,
                PlayerName = player.Name,
                GuessResult = TakeGuessResult(guessRequestDTO.Guess, playerInfo.MisteryNumber),
            };

            await UpdateGamePlayerInfoGuessAsync(playerInfo, response.GuessResult);
            await UpdateGameAfterGuessAsync(game, playerInfo);
            
            if (response.GuessResult == GuessResult.WON)
                await UpdatePlayerWinsAsync(player);

            await _repository.UpdateAsync(game);

            response.Status = game.Status;
            response.Attempts = playerInfo.Attempts;

            return response;
        }

        public async Task<GameResponseDTO> GetAsync(int id)
            => _mapper.Map<GameResponseDTO>(await _repository.GetAsync(id));

        public async Task<List<GameResponseDTO>> GetAllAsync()
        {
            var games = await _repository.GetAllAsync();

            return _mapper.Map<List<GameResponseDTO>>(games);
        }

        private async Task UpdatePlayerWinsAsync(Player player)
        {
            player.Wins++;
            await _playerRepository.UpdateAsync(player);
        }

        private async Task UpdatePlayerGames(Game game)
        {
            foreach (var playerInfos in game.GamePlayerInfos)
            {
                playerInfos.Player.GamesPlayed++;
                await _playerRepository.UpdateAsync(playerInfos.Player);
            }
        }

        private async Task UpdateGameAfterGuessAsync(Game game, GamePlayerInfo gamePlayerInfo)
        {
            var isRoundComplete = game.GamePlayerInfos.Where(w => w.Id != gamePlayerInfo.Id).All(a => a.Attempts == game.Round);

            if (gamePlayerInfo.Winner)
            {
                game.Status = isRoundComplete ? GameStatus.Finished : GameStatus.WaitingForOtherPlayers;
            }
            else
            {
                if (isRoundComplete)
                {
                    game.Status = game.Status == GameStatus.WaitingForOtherPlayers
                        ? GameStatus.Finished
                        : GameStatus.Ongoing;

                    game.Round = game.Status == GameStatus.Ongoing
                        ? game.Round + 1
                        : game.Round;
                }
            }

            if (gamePlayerInfo.Winner || isRoundComplete)
                await _repository.UpdateAsync(game);
        }

        private async Task UpdateGamePlayerInfoGuessAsync(GamePlayerInfo playerInfo, GuessResult guessResult)
        {
            playerInfo.Winner = guessResult == GuessResult.WON;
            playerInfo.Attempts++;
            await _gamePlayerInfoRepository.UpdateAsync(playerInfo);
        }

        private static GuessResult TakeGuessResult(int guess, int misteryNumber)
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

        private static void ValidateGamePlayer(Game game, GamePlayerInfo playerInfo, Player player)
        {
            if (player is null)
            {
                throw new ValidationException($"The informed player was not found.");
            }

            if (playerInfo is null)
            {
                throw new ValidationException($"The informed player (Id {player.Id} is not playing the game informed (Id {game.Id}).");
            }

            if (playerInfo.Attempts == game.Round)
            {
                throw new ValidationException("You already tried to guess the number this round! Please wait for the end of the round to continue.");
            }
        }

        private static void ValidateGuess(Game game, GuessRequestDTO gameGuessDTO)
        {
            if (gameGuessDTO.Guess < game.MinValue || gameGuessDTO.Guess > game.MaxValue)
            {
                throw new ValidationException($"The guess should be between {game.MinValue} and {game.MaxValue}");
            }
        }

        private static void ValidateGame(Game game, bool isRestart)
        {
            if (game is null)
            {
                throw new ValidationException("The informed Game was not found.");
            }

            if (isRestart && game.Status != GameStatus.Finished)
            {
                throw new ValidationException($"The Game {game.Id} is not finished. You should finish this game before restarting it.");
            }
            else if (!isRestart && game.Status == GameStatus.Finished)
            {
                throw new ValidationException($"The Game {game.Id} is finished. Note: You can restart this game!");
            }
        }

        private static void ValidateNewGameDTO(NewGameRequestDTO newGameDTO)
        {
            if (newGameDTO.MinValue <= 0)
            {
                throw new ValidationException("The minimum value should be greater than zero.");
            }

            if (newGameDTO.MaxValue <= newGameDTO.MinValue)
            {
                throw new ValidationException("The maximum value should be greater than the minimum value.");
            }
        }

        private async Task ValidateNewGamePlayersDTOAsync(List<int> playerIds)
        {
            if (!playerIds.Any())
            {
                throw new ValidationException("At least one player should be informed to start a new game.");
            }

            if (playerIds.Count != playerIds.Distinct().Count())
            {
                throw new ValidationException($"At least one player was informed more than once.");
            }

            foreach (var id in playerIds)
            {
                if (await _playerRepository.GetAsync(id) is null)
                {
                    throw new ValidationException($"No player with Id {id} was found.");
                }
            }
        }
    }
}
