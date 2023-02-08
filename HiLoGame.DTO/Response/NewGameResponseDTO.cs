namespace HiLoGame.DTO.Response
{
    public class NewGameResponseDTO : GameBaseDTO
    {
        public int Id { get; set; }
        public List<NewGamePlayerResponseDTO> Players { get; set; }
    }
}
