using HiLoGame.Crosscutting.Interfaces;
using HiLoGame.DTO.Request;
using HiLoGame.DTO.Response;
using HiLoGameAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Xunit;

namespace HiLoGame.Tests.Controller
{
    public class GameControllerTests
    {
        private readonly GameController _controller;
        private readonly Mock<IGameService> _gameServiceMock;

        public GameControllerTests()
        {
            _gameServiceMock = new Mock<IGameService>();
            _controller = new GameController(_gameServiceMock.Object);
        }

        [Fact]
        public async Task StartNewGame_ReturnsCreatedResult()
        {
            // Arrange
            var newGameRequest = new NewGameRequestDTO();
            var expectedResult = new NewGameResponseDTO();
            _gameServiceMock.Setup(x => x.StartNewGameAsync(It.IsAny<NewGameRequestDTO>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.StartNewGame(newGameRequest);

            // Assert
            var createdResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(expectedResult, createdResult.Value);
            Assert.Equal((int)HttpStatusCode.Created, createdResult.StatusCode);
        }

        [Fact]
        public async Task RestartGame_ReturnsCreatedResult()
        {
            // Arrange
            var id = 1;
            var expectedResult = new NewGameResponseDTO();
            _gameServiceMock.Setup(x => x.RestartGameAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.RestartGame(id);

            // Assert
            var createdResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(expectedResult, createdResult.Value);
            Assert.Equal((int)HttpStatusCode.Created, createdResult.StatusCode);
        }

        [Fact]
        public async Task GetGame_ReturnsOk()
        {
            //Arrange
            var id = 1;
            var result = await Task.FromResult(new GameResponseDTO());
            _gameServiceMock.Setup(s => s.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(result);

            //Act
            var actionResult = await _controller.GetGame(id);

            //Assert
            Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(result, (actionResult as OkObjectResult).Value);
        }

        [Fact]
        public async Task GetGame_ReturnsNotFound()
        {
            //Arrange
            var id = 1;
            _gameServiceMock.Setup(s => s.GetAsync(It.IsAny<int>()))
                .ReturnsAsync((GameResponseDTO)null);

            //Act
            var actionResult = await _controller.GetGame(id);

            //Assert
            Assert.IsType<NotFoundResult>(actionResult);
        }

        [Fact]
        public async Task GetGames_ReturnsNoContent()
        {
            // Arrange
            _gameServiceMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<GameResponseDTO>());

            // Act
            var result = await _controller.GetGames();

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetGames_ReturnsOk()
        {
            // Arrange
            var games = new List<GameResponseDTO> { new GameResponseDTO(), new GameResponseDTO() };
            _gameServiceMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(games);

            // Act
            var result = await _controller.GetGames();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Same(games, okResult.Value);
        }

        [Fact]
        public async Task TakeGuess_ReturnsOk()
        {
            // Arrange
            var request = new GuessRequestDTO { Guess = 5 };
            _gameServiceMock.Setup(s => s.TakeGuessAsync(It.IsAny<int>(), It.IsAny<GuessRequestDTO>()))
                .ReturnsAsync(new GuessResponseDTO());

            // Act
            var result = await _controller.TakeGuess(1, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<GuessResponseDTO>(okResult.Value);
            _gameServiceMock.Verify(s => s.TakeGuessAsync(1, request), Times.Once());
        }
    }
}
