using System.Text.Json.Serialization;

namespace HiLoGame.Crosscutting.Dtos.Response
{
    public class GameResponseDTO
    {
        public GameResponseDTO()
        {
            Players = new List<PlayerResponseDTO>();
        }

        public Guid Id { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public int Attempts { get; set; }
        [JsonIgnore]
        public bool Finished { get; set; }
        public string Status { get => Finished ? "Finished" : "Active"; }
        public List<PlayerResponseDTO> Players { get; set; }
    }
}
