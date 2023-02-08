using AutoMapper;
using HiLoGame.Common;
using HiLoGame.Common.Exceptions;
using HiLoGame.Crosscutting.Interfaces;
using HiLoGame.DTO.Request;
using HiLoGame.DTO.Response;
using HiLoGame.Entities;
using HiLoGame.Service;
using Moq;
using Xunit;

namespace HiLoGame.Tests.Service
{
    public class GameServiceTests
    {
        private readonly Mock<IGameRepository> _gameRepositoryMock;
        private readonly Mock<IGamePlayerInfoRepository> _gamePlayerInfoRepositoryMock;
        private readonly Mock<IPlayerRepository> _playerRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private GameService _gameService;

        public GameServiceTests()
        {
            _gameRepositoryMock = new Mock<IGameRepository>();
            _gamePlayerInfoRepositoryMock = new Mock<IGamePlayerInfoRepository>();
            _playerRepositoryMock = new Mock<IPlayerRepository>();
            _mapperMock = new Mock<IMapper>();

            _gameService = new GameService(_gameRepositoryMock.Object, _gamePlayerInfoRepositoryMock.Object, _playerRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllGames_ShouldReturnAllGames()
        {
            // Arrange
            var games = new List<Game>
            {
                new Game { Id = 1 },
                new Game { Id = 2 },
                new Game { Id = 3 },
            };

            var responseDto = new List<GameResponseDTO>
            {
                new GameResponseDTO { Id = 1 },
                new GameResponseDTO { Id = 2 },
                new GameResponseDTO { Id = 3 },
            };

            _gameRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(games);
            _mapperMock.Setup(m => m.Map<List<GameResponseDTO>>(games))
                .Returns(responseDto);

            // Act
            var result = await _gameService.GetAllAsync();

            // Assert
            Assert.Equal(games.Count, result.Count());
            Assert.Equal(games.Select(s => s.Id), result.Select(s => s.Id));
        }

        [Fact]
        public async Task GetGameById_ShouldReturnCorrectGame()
        {
            // Arrange
            var gameId = 1;
            var game = new Game { Id = gameId };
            _gameRepositoryMock.Setup(repo => repo.GetAsync(gameId)).ReturnsAsync(game);
            _mapperMock.Setup(m => m.Map<GameResponseDTO>(game))
                .Returns(new GameResponseDTO() { Id = gameId });

            // Act
            var result = await _gameService.GetAsync(gameId);

            // Assert
            Assert.Equal(game.Id, result.Id);
        }

        [Fact]
        public async Task GetGameById_ShouldReturnNull_WhenGameNotFound()
        {
            // Arrange
            var gameId = 1;
            _gameRepositoryMock.Setup(repo => repo.GetAsync(gameId)).ReturnsAsync((Game)null);

            // Act
            var result = await _gameService.GetAsync(gameId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task StartNewGameAsync_Success()
        {
            //Arrange
            var newGameDTO = new NewGameRequestDTO
            {
                MinValue = 1,
                MaxValue = 10,
                Players = new List<int>() { 1 }
            };

            var player1 = new Player("p1") { Id = 1, GamesPlayed = 1 };

            var game = new Game
            {
                Id = 1,
                MinValue = newGameDTO.MinValue,
                MaxValue = newGameDTO.MaxValue,
                Status = GameStatus.Ongoing,
                Round = 1,
                GamePlayerInfos = new List<GamePlayerInfo>()
                {
                    new GamePlayerInfo { PlayerId = 1, MisteryNumber = 5, Player = player1 }
                }
            };

            _mapperMock.Setup(m => m.Map<Game>(newGameDTO))
                .Returns(game);

            _mapperMock.Setup(m => m.Map<NewGameResponseDTO>(game))
                .Returns(new NewGameResponseDTO { Id = game.Id });

            _gameRepositoryMock.Setup(r => r.AddAsync(game))
                .Returns(Task.CompletedTask);

            _playerRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Player>()))
                .Returns(Task.CompletedTask);

            _playerRepositoryMock.Setup(r => r.GetAsync(1))
                .ReturnsAsync(player1);

            _playerRepositoryMock.Setup(r => r.UpdateAsync(player1))
                .Returns(Task.CompletedTask);

            //Act
            var response = await _gameService.StartNewGameAsync(newGameDTO);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(game.Id, response.Id);
            _gameRepositoryMock.Verify(r => r.AddAsync(game), Times.Once);
            _playerRepositoryMock.Verify(r => r.GetAsync(1), Times.Once);
            _playerRepositoryMock.Verify(r => r.UpdateAsync(player1), Times.Once);
        }

        [Fact]
        public async Task StartNewGameAsync_MinValueLessThanOne_ThrowsValidationException()
        {
            //Arrange
            var newGameDTO = new NewGameRequestDTO
            {
                MinValue = 0,
                MaxValue = 10,
                Players = new List<int>() { 1 }
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _gameService.StartNewGameAsync(newGameDTO));

            _gameRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Game>()), Times.Never);
            _playerRepositoryMock.Verify(r => r.GetAsync(1), Times.Never);
            _playerRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Player>()), Times.Never);
        }

        [Fact]
        public async Task StartNewGameAsync_MaxValueLessThanMinimum_ThrowsValidationException()
        {
            //Arrange
            var newGameDTO = new NewGameRequestDTO
            {
                MinValue = 1,
                MaxValue = -2,
                Players = new List<int>() { 1 }
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _gameService.StartNewGameAsync(newGameDTO));

            _gameRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Game>()), Times.Never);
            _playerRepositoryMock.Verify(r => r.GetAsync(1), Times.Never);
            _playerRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Player>()), Times.Never);
        }

        [Fact]
        public async Task StartNewGameAsync_NoPlayersInformed_ThrowsValidationException()
        {
            //Arrange
            var newGameDTO = new NewGameRequestDTO
            {
                MinValue = 1,
                MaxValue = 10,
                Players = new List<int>()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _gameService.StartNewGameAsync(newGameDTO));

            _gameRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Game>()), Times.Never);
            _playerRepositoryMock.Verify(r => r.GetAsync(1), Times.Never);
            _playerRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Player>()), Times.Never);
        }

        [Fact]
        public async Task StartNewGameAsync_SamePlayerInformedTwice_ThrowsValidationException()
        {
            //Arrange
            var newGameDTO = new NewGameRequestDTO
            {
                MinValue = 1,
                MaxValue = 10,
                Players = new List<int>() { 1, 1 }
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _gameService.StartNewGameAsync(newGameDTO));

            _playerRepositoryMock.Verify(r => r.GetAsync(1), Times.Never);
            _gameRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Game>()), Times.Never);
            _playerRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Player>()), Times.Never);
        }

        [Fact]
        public async Task StartNewGameAsync_PlayerNotFound_ThrowsValidationException()
        {
            //Arrange
            var newGameDTO = new NewGameRequestDTO
            {
                MinValue = 1,
                MaxValue = 10,
                Players = new List<int>() { 1 }
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _gameService.StartNewGameAsync(newGameDTO));

            _playerRepositoryMock.Verify(r => r.GetAsync(1), Times.Once);
            _gameRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Game>()), Times.Never);
            _playerRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Player>()), Times.Never);
        }

        [Fact]
        public async Task RestartGameAsync_ValidInput_ReturnsNewGameResponseDTO()
        {
            // Arrange
            var gameId = 1;
            var game = new Game
            {
                Id = gameId,
                Status = GameStatus.Finished
            };

            var newGame = new Game
            {
                Id = gameId + 1,
                Status = GameStatus.Ongoing
            };

            _gameRepositoryMock.Setup(x => x.GetAsync(gameId)).ReturnsAsync(game);
            _gameRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Game>()));

            _mapperMock.Setup(x => x.Map<Game>(game)).Returns(newGame);
            _mapperMock.Setup(x => x.Map<NewGameResponseDTO>(newGame)).Returns(new NewGameResponseDTO { Id = newGame.Id });

            // Act
            var result = await _gameService.RestartGameAsync(gameId);

            // Assert
            _gameRepositoryMock.Verify(x => x.GetAsync(gameId), Times.Once);
            _gameRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Game>()), Times.Once);
            _mapperMock.Verify(x => x.Map<Game>(game), Times.Once);
            _mapperMock.Verify(x => x.Map<NewGameResponseDTO>(newGame), Times.Once);
            Assert.Equal(newGame.Id, result.Id);
        }

        [Fact]
        public async Task RestartGameAsync_GameNotFound_ThrowsValidationException()
        {
            // Arrange
            var gameId = 1;
            _gameRepositoryMock.Setup(x => x.GetAsync(gameId)).ReturnsAsync((Game)null);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _gameService.RestartGameAsync(gameId));

            _gameRepositoryMock.Verify(x => x.GetAsync(gameId), Times.Once);
            _gameRepositoryMock.VerifyNoOtherCalls();
            _mapperMock.VerifyNoOtherCalls();
            _playerRepositoryMock.VerifyNoOtherCalls();
            _gamePlayerInfoRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task RestartGameAsync_GameNotFinished_ThrowsValidationException()
        {
            // Arrange
            var gameId = 1;
            var game = new Game()
            {
                Id = gameId,
                Status = GameStatus.Ongoing
            };

            _gameRepositoryMock.Setup(x => x.GetAsync(gameId)).ReturnsAsync(game);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _gameService.RestartGameAsync(gameId));

            _gameRepositoryMock.Verify(x => x.GetAsync(gameId), Times.Once);
            _gameRepositoryMock.VerifyNoOtherCalls();
            _mapperMock.VerifyNoOtherCalls();
            _playerRepositoryMock.VerifyNoOtherCalls();
            _gamePlayerInfoRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task TakeGuessAsync_Should_ReturnLOGuessResult()
        {
            // Arrange
            var gameId = 1;
            var guessRequestDTO = new GuessRequestDTO { PlayerId = 1, Guess = 7 };
            var game = new Game { Id = gameId, Status = GameStatus.Ongoing, Round = 1, MinValue = 1, MaxValue = 10 };
            var player = new Player("John Doe") { Id = 1 };
            var playerInfo = new GamePlayerInfo { PlayerId = 1, MisteryNumber = 5, Winner = false, Attempts = 0 };
            game.GamePlayerInfos = new List<GamePlayerInfo> { playerInfo };

            _gameRepositoryMock.Setup(x => x.GetAsync(gameId)).ReturnsAsync(game);
            _playerRepositoryMock.Setup(x => x.GetAsync(guessRequestDTO.PlayerId)).ReturnsAsync(player);

            // Act
            var result = await _gameService.TakeGuessAsync(gameId, guessRequestDTO);

            // Assert
            Assert.Equal(GuessResult.LO, result.GuessResult);
            Assert.Equal(gameId, result.GameId);
            Assert.Equal(player.Id, result.PlayerId);
            Assert.Equal(player.Name, result.PlayerName);
            Assert.Equal(1, result.Attempts);
            Assert.Equal(GameStatus.Ongoing, result.Status);
        }

        [Fact]
        public async Task TakeGuessAsync_Should_ReturnHIGuessResult()
        {
            // Arrange
            var gameId = 1;
            var guessRequestDTO = new GuessRequestDTO { PlayerId = 1, Guess = 5 };
            var game = new Game { Id = gameId, Status = GameStatus.Ongoing, Round = 1, MinValue = 1, MaxValue = 10 };
            var player = new Player("John Doe") { Id = 1 };
            var playerInfo = new GamePlayerInfo { PlayerId = 1, MisteryNumber = 7, Winner = false, Attempts = 0 };
            game.GamePlayerInfos = new List<GamePlayerInfo> { playerInfo };

            _gameRepositoryMock.Setup(x => x.GetAsync(gameId)).ReturnsAsync(game);
            _playerRepositoryMock.Setup(x => x.GetAsync(guessRequestDTO.PlayerId)).ReturnsAsync(player);

            // Act
            var result = await _gameService.TakeGuessAsync(gameId, guessRequestDTO);

            // Assert
            Assert.Equal(GuessResult.HI, result.GuessResult);
            Assert.Equal(gameId, result.GameId);
            Assert.Equal(player.Id, result.PlayerId);
            Assert.Equal(player.Name, result.PlayerName);
            Assert.Equal(1, result.Attempts);
            Assert.Equal(GameStatus.Ongoing, result.Status);
        }

        [Fact]
        public async Task TakeGuessAsync_Should_ReturnWONGuessResult()
        {
            // Arrange
            var gameId = 1;
            var guessRequestDTO = new GuessRequestDTO { PlayerId = 1, Guess = 5 };
            var game = new Game { Id = gameId, Status = GameStatus.Ongoing, Round = 1, MinValue = 1, MaxValue = 10 };
            var player = new Player("John Doe") { Id = 1 };
            var playerInfo = new GamePlayerInfo { PlayerId = 1, MisteryNumber = 5, Winner = true, Attempts = 0 };
            game.GamePlayerInfos = new List<GamePlayerInfo> { playerInfo };

            _gameRepositoryMock.Setup(x => x.GetAsync(gameId)).ReturnsAsync(game);
            _playerRepositoryMock.Setup(x => x.GetAsync(guessRequestDTO.PlayerId)).ReturnsAsync(player);

            // Act
            var result = await _gameService.TakeGuessAsync(gameId, guessRequestDTO);

            // Assert
            Assert.Equal(GuessResult.WON, result.GuessResult);
            Assert.Equal(gameId, result.GameId);
            Assert.Equal(player.Id, result.PlayerId);
            Assert.Equal(player.Name, result.PlayerName);
            Assert.Equal(1, result.Attempts);
            Assert.Equal(GameStatus.Finished, result.Status);
        }

        [Fact]
        public async Task TakeGuessAsync_InvalidGuess_ThrowsValidationException()
        {
            // Arrange
            var gameId = 1;
            var guessRequestDTO = new GuessRequestDTO { PlayerId = 1, Guess = 50 };
            var game = new Game { Id = gameId, Status = GameStatus.Ongoing, Round = 1, MinValue = 1, MaxValue = 10 };
            var player = new Player("John Doe") { Id = 1 };
            var playerInfo = new GamePlayerInfo { PlayerId = 1, MisteryNumber = 5, Winner = true, Attempts = 0 };
            game.GamePlayerInfos = new List<GamePlayerInfo> { playerInfo };

            _gameRepositoryMock.Setup(x => x.GetAsync(gameId)).ReturnsAsync(game);
            _playerRepositoryMock.Setup(x => x.GetAsync(guessRequestDTO.PlayerId)).ReturnsAsync(player);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _gameService.TakeGuessAsync(gameId, guessRequestDTO));

            _gameRepositoryMock.Verify(x => x.GetAsync(gameId), Times.Once);
            _gameRepositoryMock.VerifyNoOtherCalls();
            _playerRepositoryMock.Verify(x => x.GetAsync(gameId), Times.Once);
            _playerRepositoryMock.VerifyNoOtherCalls();
            _mapperMock.VerifyNoOtherCalls();
            _gamePlayerInfoRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task TakeGuessAsync_GameIsFinished_ThrowsValidationException()
        {
            // Arrange
            var gameId = 1;
            var guessRequestDTO = new GuessRequestDTO { PlayerId = 1, Guess = 5 };
            var game = new Game { Id = gameId, Status = GameStatus.Finished, Round = 1, MinValue = 1, MaxValue = 10 };

            _gameRepositoryMock.Setup(x => x.GetAsync(gameId)).ReturnsAsync(game);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _gameService.TakeGuessAsync(gameId, guessRequestDTO));

            _gameRepositoryMock.Verify(x => x.GetAsync(gameId), Times.Once);
            _gameRepositoryMock.VerifyNoOtherCalls();
            _playerRepositoryMock.VerifyNoOtherCalls();
            _mapperMock.VerifyNoOtherCalls();
            _gamePlayerInfoRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task TakeGuessAsync_PlayerNotFound_ThrowsValidationException()
        {
            // Arrange
            var gameId = 1;
            var guessRequestDTO = new GuessRequestDTO { PlayerId = 1, Guess = 6 };
            var game = new Game { Id = gameId, Status = GameStatus.Ongoing, Round = 1, MinValue = 1, MaxValue = 10 };
            Player player = null;
            var playerInfo = new GamePlayerInfo { PlayerId = 0, MisteryNumber = 5, Winner = false, Attempts = 0 };
            game.GamePlayerInfos = new List<GamePlayerInfo> { playerInfo };

            _gameRepositoryMock.Setup(x => x.GetAsync(gameId)).ReturnsAsync(game);
            _playerRepositoryMock.Setup(x => x.GetAsync(guessRequestDTO.PlayerId)).ReturnsAsync(player);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _gameService.TakeGuessAsync(gameId, guessRequestDTO));

            _gameRepositoryMock.Verify(x => x.GetAsync(gameId), Times.Once);
            _gameRepositoryMock.VerifyNoOtherCalls();
            _playerRepositoryMock.Verify(x => x.GetAsync(gameId), Times.Once);
            _playerRepositoryMock.VerifyNoOtherCalls();
            _mapperMock.VerifyNoOtherCalls();
            _gamePlayerInfoRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task TakeGuessAsync_PlayerNotPlayingThisGame_ThrowsValidationException()
        {
            // Arrange
            var gameId = 1;
            var guessRequestDTO = new GuessRequestDTO { PlayerId = 1, Guess = 6 };
            var game = new Game { Id = gameId, Status = GameStatus.Ongoing, Round = 1, MinValue = 1, MaxValue = 10 };
            var player = new Player("John Doe") { Id = 1 };
            var playerInfo = new GamePlayerInfo();
            game.GamePlayerInfos = new List<GamePlayerInfo> { playerInfo };

            _gameRepositoryMock.Setup(x => x.GetAsync(gameId)).ReturnsAsync(game);
            _playerRepositoryMock.Setup(x => x.GetAsync(guessRequestDTO.PlayerId)).ReturnsAsync(player);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _gameService.TakeGuessAsync(gameId, guessRequestDTO));

            _gameRepositoryMock.Verify(x => x.GetAsync(gameId), Times.Once);
            _gameRepositoryMock.VerifyNoOtherCalls();
            _playerRepositoryMock.Verify(x => x.GetAsync(gameId), Times.Once);
            _playerRepositoryMock.VerifyNoOtherCalls();
            _mapperMock.VerifyNoOtherCalls();
            _gamePlayerInfoRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task TakeGuessAsync_PlayerAlreadyPlayedThisRound_ThrowsValidationException()
        {
            // Arrange
            var gameId = 1;
            var guessRequestDTO = new GuessRequestDTO { PlayerId = 1, Guess = 6 };
            var game = new Game { Id = gameId, Status = GameStatus.Ongoing, Round = 1, MinValue = 1, MaxValue = 10 };
            var player = new Player("John Doe") { Id = 1 };
            var playerInfo = new GamePlayerInfo { PlayerId = 1, MisteryNumber = 5, Winner = false, Attempts = 1 };
            game.GamePlayerInfos = new List<GamePlayerInfo> { playerInfo };

            _gameRepositoryMock.Setup(x => x.GetAsync(gameId)).ReturnsAsync(game);
            _playerRepositoryMock.Setup(x => x.GetAsync(guessRequestDTO.PlayerId)).ReturnsAsync(player);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _gameService.TakeGuessAsync(gameId, guessRequestDTO));

            _gameRepositoryMock.Verify(x => x.GetAsync(gameId), Times.Once);
            _gameRepositoryMock.VerifyNoOtherCalls();
            _playerRepositoryMock.Verify(x => x.GetAsync(gameId), Times.Once);
            _playerRepositoryMock.VerifyNoOtherCalls();
            _mapperMock.VerifyNoOtherCalls();
            _gamePlayerInfoRepositoryMock.VerifyNoOtherCalls();
        }
    }
}
