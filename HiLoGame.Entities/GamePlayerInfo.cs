namespace HiLoGame.Entities
{
    public class GamePlayerInfo : BaseEntity
    {
        public int GameId { get; set; }

        public int PlayerId { get; set; }

        public int MisteryNumber { get; set; }

        public int Attempts { get; set; }

        public bool Winner { get; set; }

        public Player Player { get; set; }
    }
}