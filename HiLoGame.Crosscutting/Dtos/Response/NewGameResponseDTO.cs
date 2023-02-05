namespace HiLoGame.Crosscutting.Dtos.Response
{
    public class NewGameResponseDTO : NewGameDTO
    {
        public Guid Id { get; set; }
        public List<PlayerDTO> Players { get; set; }
    }
}
