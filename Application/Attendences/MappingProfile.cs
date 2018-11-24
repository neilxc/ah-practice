using AutoMapper;
using Domain;

namespace Application.Attendences
 {
     public class MappingProfile : Profile
     {
         public MappingProfile()
         {
             CreateMap<ActivityAttendee, AttendeeDTO>()
                 .ForMember(d => d.Username, o => o.MapFrom(s => s.AppUser.UserName));
         }
     }
 }