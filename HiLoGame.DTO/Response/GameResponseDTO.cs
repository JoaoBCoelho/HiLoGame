namespace HiLoGame.DTO.Response
{
    public class GameResponseDTO : GameBaseDTO
    {
        public GameResponseDTO()
        {
            GamePlayerInfos = new List<GamePlayerInfoDTO>();
        }

        public int Id { get; set; }
        public int Round { get; set; }
        public string Status { get; set; }
        public List<GamePlayerInfoDTO> GamePlayerInfos { get; set; }
    }
}
