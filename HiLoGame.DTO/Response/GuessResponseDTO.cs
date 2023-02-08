using HiLoGame.Common;
using System.Text.Json.Serialization;

namespace HiLoGame.DTO.Response
{
    public class GuessResponseDTO
    {
        public int GameId { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int Attempts { get; set; }
        public string Result { get => GuessResult.ToString(); }
        public string GameStatus { get => Status.ToString(); }
        [JsonIgnore]
        public GuessResult GuessResult { get; set; }
        [JsonIgnore]
        public GameStatus Status { get; set; }
    }
}
