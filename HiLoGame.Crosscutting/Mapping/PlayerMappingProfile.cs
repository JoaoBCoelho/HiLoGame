using AutoMapper;
using HiLoGame.DTO.Response;
using HiLoGame.Entities;

namespace HiLoGame.Crosscutting.Mapping
{
    public class PlayerMappingProfile : Profile
    {
        public PlayerMappingProfile()
        {

            CreateMap<Player, PlayerDTO>().ReverseMap();
        }
    }
}
