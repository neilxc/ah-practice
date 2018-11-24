using System.Collections.Generic;
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
            CreateMap<List<ActivityAttendee>, List<AttendeeDTO>>();
        }
    }
}