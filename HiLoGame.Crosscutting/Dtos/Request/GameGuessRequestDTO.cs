namespace HiLoGame.Crosscutting.Dtos.Request
{
    public class GameGuessRequestDTO
    {
        public Guid GameId { get; set; }
        public List<PlayerGuessRequestDTO> PlayerGuesses { get; set; }
    }
}