using AutoMapper;
using HiLoGame.Common.Exceptions;
using HiLoGame.Crosscutting.Interfaces;
using HiLoGame.Crosscutting.Mapping;
using HiLoGame.DTO.Response;
using HiLoGame.Entities;
using HiLoGame.Service;
using Moq;
using Xunit;

namespace HiLoGame.Tests.Service
{
    public class PlayerServiceTests
    {
        private readonly PlayerService _playerService;
        private readonly Mock<IPlayerRepository> _playerRepositoryMock;

        public PlayerServiceTests()
        {
            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new PlayerMappingProfile()));
            var mapper = mapperConfig.CreateMapper();

            _playerRepositoryMock = new Mock<IPlayerRepository>();
            _playerService = new PlayerService(mapper, _playerRepositoryMock.Object);
        }

        [Fact]
        public async Task CreatePlayerAsync_WithInvalidName_ThrowsValidationException()
        {
            // Arrange
            var invalidName = "";

            // Act
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _playerService.CreatePlayerAsync(invalidName));

            // Assert
            Assert.Equal("The player name must be informed.", exception.Message);
        }

        [Fact]
        public async Task CreatePlayerAsync_WithValidName_AddsPlayerToRepository()
        {
            // Arrange
            var validName = "John Doe";
            var player = new Player(validName);
            var playerDTO = new PlayerDTO() { Name = validName };

            _playerRepositoryMock.Setup(x => x.AddAsync(player));

            // Act
            var result = await _playerService.CreatePlayerAsync(validName);

            // Assert
            _playerRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Player>()), Times.Once());
            Assert.Equal(validName, result.Name);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsPlayers()
        {
            // Arrange
            var players = new List<Player>()
            {
                new Player("John Doe"),
                new Player("Jane Doe")
            };

            var playersDTO = new List<PlayerDTO>()
            {
                new PlayerDTO() {Name = "John Doe" },
                new PlayerDTO() {Name = "Jane Doe" }
            };

            _playerRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(players);

            // Act
            var result = await _playerService.GetAllAsync();

            // Assert
            _playerRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once());
            Assert.Equal(players.Count, result.Count);
            Assert.Equal(playersDTO.Select(s => s.Name), result.Select(s => s.Name));
        }

        [Fact]
        public async Task GetAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var invalidId = 0;

            // Act
            var result = await _playerService.GetAsync(invalidId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsListOfPlayers()
        {
            // Arrange
            var players = new List<Player> {
                new Player("player 1"),
                new Player("player 2"),
                new Player("player 3"),
            };

            _playerRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(players);

            // Act
            var result = await _playerService.GetAllAsync();

            // Assert
            _playerRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once());
            Assert.Equal(players.Count, result.Count);
            Assert.Equal(players.Select(s => s.Name), result.Select(s => s.Name));
        }

        [Fact]
        public async Task GetAllAsync_WithEmptyList_ReturnsEmptyList()
        {
            // Arrange
            var players = new List<Player>();
            _playerRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(players);

            // Act
            var result = await _playerService.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }
    }
}
