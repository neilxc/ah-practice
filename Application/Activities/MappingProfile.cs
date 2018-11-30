using System.Collections.Generic;
using System.Linq;
using Application.Attendences;
using AutoMapper;
using Domain;

namespace Application.Activities
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Create.ActivityData, Activity>();
            CreateMap<Activity, ActivityDTO>();
            CreateMap<ActivityAttendee, AttendeeDTO>()
                .ForMember(d => d.Username, o => o.MapFrom(s => s.AppUser.UserName))
                .ForMember(d => d.Image, o => o.MapFrom(s => s.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url));
        }
    }
}