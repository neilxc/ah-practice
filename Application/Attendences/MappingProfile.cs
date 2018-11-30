using System.Linq;
using AutoMapper;
using Domain;

namespace Application.Attendences
 {
     public class MappingProfile : Profile
     {
         public MappingProfile()
         {
             CreateMap<ActivityAttendee, AttendeeDTO>()
                 .ForMember(d => d.Username, o => o.MapFrom(s => s.AppUser.UserName))
                 .ForMember(d => d.Image, o => o.MapFrom(s => s.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url));
         }
     }
 }