using HiLoGame.Common;

namespace HiLoGame.Entities
{
    public class Game : BaseEntity
    {
        public Game()
        {
            GamePlayerInfos = new List<GamePlayerInfo>();
        }

        public int MinValue { get; set; }

        public int MaxValue { get; set; }

        public int Round { get; set; }

        public GameStatus Status { get; set; }

        public List<GamePlayerInfo> GamePlayerInfos { get; set; }
    }
}