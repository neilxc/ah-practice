using System.Linq;
using AutoMapper;
using Domain;

namespace Application.Users
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AppUser, User>()
                .ForMember(d => d.Image, o => o
                    .MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain).Url));
        }
    }
}