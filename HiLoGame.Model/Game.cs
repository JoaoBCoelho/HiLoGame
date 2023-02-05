using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HiLoGame.Model
{
    public class Game
    {
        public Game()
        {
            GameInstances = new List<GameInstance>();
        }

        [BsonId]
        [BsonRepresentation(BsonType.Binary)]
        public Guid Id { get; set; }

        [BsonElement("MinValue")]
        public int MinValue { get; set; }

        [BsonElement("MaxValue")]
        public int MaxValue { get; set; }

        [BsonElement("Attempts")]
        public int Attempts { get; set; }

        [BsonElement("Finished")]
        public bool Finished { get; set; }

        [BsonElement("GameInstances")]
        public List<GameInstance> GameInstances { get; set; }
    }
}