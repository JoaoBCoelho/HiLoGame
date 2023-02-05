using AutoMapper;
using HiLoGame.Crosscutting.Dtos.Request;
using HiLoGame.Crosscutting.Dtos.Response;
using HiLoGame.Model;

namespace HiLoGame.Crosscutting.Mapping
{
    public class GameMappingProfile : Profile
    {
        public GameMappingProfile()
        {
            CreateMap<NewGameRequestDTO, Game>()
                .ForMember(dest => dest.GameInstances, opt => opt.MapFrom(src =>
                    src.Players.Select(name =>
                    new GameInstance
                    {
                        Id = Guid.NewGuid(),
                        PlayerName = name,
                        MisteryNumber = new Random().Next(src.MinValue, src.MaxValue + 1)
                    })
                ));

            CreateMap<Game, Game>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Attempts, opt => opt.Ignore())
                .ForMember(dest => dest.Finished, opt => opt.Ignore())
                .ForMember(dest => dest.GameInstances, opt => opt.MapFrom(src =>
                    src.GameInstances.Select(instance =>
                    new GameInstance
                    {
                        Id = Guid.NewGuid(),
                        PlayerName = instance.PlayerName,
                        MisteryNumber = new Random().Next(src.MinValue, src.MaxValue + 1),
                        Winner = false
                    })
                ));

            CreateMap<Game, Game>()
                .ForMember(dest => dest.GameInstances, opt => opt.MapFrom(src => src.GameInstances));

            CreateMap<GameInstance, PlayerDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.PlayerName));

            CreateMap<GameInstance, PlayerResponseDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.PlayerName))
                .ForMember(dest => dest.Winner, opt => opt.MapFrom(src => src.Winner));

            CreateMap<Game, GameResponseDTO>()
                .ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.GameInstances));
        }
    }
}
