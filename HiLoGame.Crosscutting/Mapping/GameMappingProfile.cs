using AutoMapper;
using HiLoGame.DTO.Request;
using HiLoGame.DTO.Response;
using HiLoGame.Entities;

namespace HiLoGame.Crosscutting.Mapping
{
    public class GameMappingProfile : Profile
    {
        public GameMappingProfile()
        {
            CreateMap<NewGameRequestDTO, Game>()
                .ForMember(dest => dest.Round, opt => opt.MapFrom(src => 1))
                .ForMember(dest => dest.GamePlayerInfos, opt => opt.MapFrom(src =>
                    src.Players.Select(id =>
                    new GamePlayerInfo
                    {
                        PlayerId = id,
                        MisteryNumber = new Random().Next(src.MinValue, src.MaxValue + 1)
                    })
                ));

            CreateMap<Game, Game>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Round, opt => opt.MapFrom(src => 1))
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.GamePlayerInfos, opt => opt.MapFrom(src =>
                    src.GamePlayerInfos.Select(playerInfo =>
                    new GamePlayerInfo
                    {
                        PlayerId = playerInfo.PlayerId,
                        MisteryNumber = new Random().Next(src.MinValue, src.MaxValue + 1),
                        Winner = false
                    })
                ));

            CreateMap<Game, NewGameResponseDTO>()
                .ForMember(dest => dest.Players, opt => opt.MapFrom(src =>
                    src.GamePlayerInfos.Select(playerInfo =>
                    new NewGamePlayerResponseDTO
                    {
                        Id = playerInfo.Player.Id,
                        Name = playerInfo.Player.Name
                    })
                ));

            CreateMap<Game, GameResponseDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.GamePlayerInfos, opt => opt.MapFrom(src =>
                    src.GamePlayerInfos.Select(playerInfo =>
                    new GamePlayerInfoDTO
                    {
                        GameId = src.Id,
                        PlayerId = playerInfo.PlayerId,
                        PlayerName = playerInfo.Player.Name,
                        Attempts = playerInfo.Attempts,
                        Winner = playerInfo.Winner
                    })
                ));
        }
    }
}
