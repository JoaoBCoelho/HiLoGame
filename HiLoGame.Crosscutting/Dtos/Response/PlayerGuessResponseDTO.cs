using System.Text.Json.Serialization;

namespace HiLoGame.Crosscutting.Dtos.Response
{
    public class PlayerGuessResponseDTO : PlayerDTO
    {
        [JsonIgnore]
        public GuessResult GuessResult { get; set; }
        public string Result { get => GuessResult.ToString(); }
    }
}
