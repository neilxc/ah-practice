using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class Create
    {
        public class ActivityData
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime Date { get; set; }
            public string City { get; set; }
            public string Venue { get; set; }
            public GeoCoordinate GeoCoordinate { get; set; }
        }

        public class ActivityDataValidator : AbstractValidator<ActivityData>
        {
            public ActivityDataValidator()
            {
                RuleFor(x => x.Title).NotEmpty().NotNull();
                RuleFor(x => x.Description).NotEmpty().NotNull();
                RuleFor(x => x.Date).NotEmpty().NotNull();
                RuleFor(x => x.City).NotEmpty().NotNull();
                RuleFor(x => x.Venue).NotEmpty().NotNull();
                RuleFor(x => x.GeoCoordinate.Latitude).NotEmpty().NotNull();
                RuleFor(x => x.GeoCoordinate.Longitude).NotEmpty().NotNull();
            }
        }
        
        public class Command : IRequest<ActivityDTO>
        {
            public ActivityData Activity { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Activity).NotNull().SetValidator(new ActivityDataValidator());
            }
        }

        public class Handler : IRequestHandler<Command, ActivityDTO>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IUserAccessor userAccessor, IMapper mapper)
            {
                _context = context;
                _userAccessor = userAccessor;
                _mapper = mapper;
            }
            
            public async Task<ActivityDTO> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = _mapper.Map<ActivityData, Activity>(request.Activity);
                
                var username = _userAccessor.GetCurrentUsername();
                
                var user = await _context.Users.
                    FirstOrDefaultAsync(x => x.UserName == username, cancellationToken);

                await _context.Activities.AddAsync(activity, cancellationToken);

                var attendee = new ActivityAttendee
                {
                    AppUser = user,
                    Activity = activity,
                    DateJoined = DateTime.Now,
                    IsHost = true,
                    ActivityId = activity.Id,
                    AppUserId = user.Id
                };

                await _context.ActivityAttendees.AddAsync(attendee, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                var activityToReturn = _mapper.Map<Activity, ActivityDTO>(activity);

                return activityToReturn;
            }
        }
    }
}