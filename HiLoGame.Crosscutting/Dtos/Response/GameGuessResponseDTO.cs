namespace HiLoGame.Crosscutting.Dtos.Response
{
    public class GameGuessResponseDTO
    {
        public GameGuessResponseDTO(Guid gameId)
        {
            GameId = gameId;
            PlayerGuessesResponse = new List<PlayerGuessResponseDTO>();
        }

        public Guid GameId { get; set; }
        public List<PlayerGuessResponseDTO> PlayerGuessesResponse { get; set; }
    }
}
