using AutoMapper;
using Server.Dtos;
using Server.Models;

namespace Server.Profiles
{
    public class DataloggerProfile : Profile
    {
        public DataloggerProfile()
        {
            CreateMap<Datalogger, DataloggerDto>();
        }
    }
}
