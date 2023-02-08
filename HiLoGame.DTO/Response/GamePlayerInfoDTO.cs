namespace HiLoGame.DTO.Response
{
    public class GamePlayerInfoDTO
    {
        public int GameId { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int Attempts { get; set; }
        public bool Winner { get; set; }
    }
}
