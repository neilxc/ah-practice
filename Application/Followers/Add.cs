using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using Application.Profiles;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers
{
    public class Add
    {
        public class Command : IRequest<Profile>
        {
            public Command(string username)
            {
                Username = username;
            }

            public string Username { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Username).NotNull().NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, Profile>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            private readonly IProfileReader _profileReader;

            public Handler(DataContext context, IUserAccessor userAccessor, IProfileReader profileReader)
            {
                _context = context;
                _userAccessor = userAccessor;
                _profileReader = profileReader;
            }
            
            public async Task<Profile> Handle(Command request, CancellationToken cancellationToken)
            {
                var target =
                    await _context.Users.FirstOrDefaultAsync(x => x.UserName == request.Username,
                        cancellationToken);
                
                if (target == null)
                    throw new RestException(HttpStatusCode.NotFound, new {User = "Not found"});

                var observer =
                    await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetCurrentUsername(),
                        cancellationToken);

                var followedPeople =
                    await _context.FollowedPeople.FirstOrDefaultAsync(
                        x => x.ObserverId == observer.Id && x.TargetId == target.Id, cancellationToken);

                if (followedPeople == null)
                {
                    followedPeople = new FollowedPeople
                    {
                        Observer = observer,
                        ObserverId = observer.Id,
                        Target = target,
                        TargetId = target.Id
                    };

                    await _context.FollowedPeople.AddAsync(followedPeople, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return await _profileReader.ReadProfile(request.Username);
            }
        }
    }
}