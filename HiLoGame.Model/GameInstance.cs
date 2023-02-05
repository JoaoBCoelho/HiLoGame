using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace HiLoGame.Model
{
    public class GameInstance
    {
        [BsonId]
        [BsonRepresentation(BsonType.Binary)]
        public Guid Id { get; set; }

        [BsonElement("PlayerName")]
        public string PlayerName { get; set; }

        [BsonElement("MisteryNumber")]
        public int MisteryNumber { get; set; }

        [BsonElement("Winner")]
        public bool Winner { get; set; }
    }
}