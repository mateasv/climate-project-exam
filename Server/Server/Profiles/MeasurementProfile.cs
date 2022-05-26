// https://code-maze.com/automapper-net-core/

using AutoMapper;
using Server.Dtos;
using Server.Models;

namespace Server.Profiles
{
    public class MeasurementProfile : Profile
    {
        public MeasurementProfile()
        {
            CreateMap<Measurement, MeasurementDto>();
        }
    }
}
