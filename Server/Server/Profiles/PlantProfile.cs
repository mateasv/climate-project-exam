
using AutoMapper;
using Server.Dtos;
using Server.Models;

namespace Server.Profiles
{
    public class PlantProfile : Profile
    {
        public PlantProfile()
        {
            CreateMap<Plant, PlantDto>();
        }
    }
}
