using AutoMapper;
using Domain;

namespace Application.Comments
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Comment, CommentDto>();
            CreateMap<AppUser, Profiles.Profile>();
        }
    }
}