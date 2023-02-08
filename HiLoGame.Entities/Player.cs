namespace HiLoGame.Entities
{
    public class Player : BaseEntity
    {
        public Player(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public int GamesPlayed { get; set; }

        public int Wins { get; set; }
    }
}
