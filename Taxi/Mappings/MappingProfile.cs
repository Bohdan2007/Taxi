using AutoMapper;
using Taxi.DTOs;
using Taxi.Models;
using Taxi.Validators;

namespace Taxi.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Car, CarResponseDto>()
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => DateTime.UtcNow.Year - src.Year));

        CreateMap<CarBaseDto, Car>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Drivers, opt => opt.Ignore())
            .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand.Trim()))
            .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model.Trim()));
            
        CreateMap<CarCreateDto, Car>().IncludeBase<CarBaseDto, Car>();
        CreateMap<CarUpdateDto, Car>().IncludeBase<CarBaseDto, Car>();

        CreateMap<Driver, DriverResponseDto>();

        CreateMap<DriverBaseDto, Driver>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Car, opt => opt.Ignore())
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName.Trim()));
            
        CreateMap<DriverCreateDto, Driver>().IncludeBase<DriverBaseDto, Driver>();
        CreateMap<DriverUpdateDto, Driver>().IncludeBase<DriverBaseDto, Driver>();
    }
}
