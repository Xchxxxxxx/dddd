using AutoMapper;
using DDD.Application.DTOs;
using DDD.Domain.UserAggregate;

namespace DDD.Application.Mappings;

public sealed class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.Email, o => o.MapFrom(s => s.Email.Value))
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));
    }
}