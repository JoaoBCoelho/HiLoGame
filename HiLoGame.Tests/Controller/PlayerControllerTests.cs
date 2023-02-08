using HiLoGame.Crosscutting.Interfaces;
using HiLoGame.DTO.Response;
using HiLoGameAPI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HiLoGame.Tests.Controller
{
    public class PlayerControllerTests
    {
        private readonly PlayerController _controller;
        private readonly Mock<IPlayerService> _playerServiceMock;

        public PlayerControllerTests()
        {
            _playerServiceMock = new Mock<IPlayerService>();
            _controller = new PlayerController(_playerServiceMock.Object);
        }

        [Fact]
        public async Task CreatePlayer_ReturnsCreated()
        {
            // Arrange
            var expectedResult = new PlayerDTO { Id = 1, Name = "Player 1" };
            _playerServiceMock.Setup(x => x.CreatePlayerAsync(It.IsAny<string>())).ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CreatePlayer("Player 1");

            // Assert
            Assert.NotNull(result);
            var createdResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
            Assert.Equal(expectedResult, createdResult.Value);
        }

        [Fact]
        public async Task GetPlayer_ReturnsNotFound()
        {
            // Arrange
            _playerServiceMock.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync((PlayerDTO)null);

            // Act
            var result = await _controller.GetPlayer(1);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetPlayer_ReturnsOk()
        {
            // Arrange
            var expectedResult = new PlayerDTO { Id = 1, Name = "Player 1" };
            _playerServiceMock.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetPlayer(1);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }

        [Fact]
        public async Task GetPlayers_ReturnsNoContent()
        {
            // Arrange
            _playerServiceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<PlayerDTO>());

            // Act
            var result = await _controller.GetPlayers();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetPlayers_ReturnsOk()
        {
            // Arrange
            var expectedResult = new List<PlayerDTO>
            {
                new PlayerDTO { Id = 1, Name = "Player 1" },
                new PlayerDTO { Id = 2, Name = "Player 2" }
            };

            var playerServiceMock = new Mock<IPlayerService>();
            playerServiceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(expectedResult);
            var controller = new PlayerController(playerServiceMock.Object);

            // Act
            var result = await controller.GetPlayers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(result);
            Assert.Equal(expectedResult, okResult.Value);
        }
    }
}
