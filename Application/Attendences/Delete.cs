using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Activities;
using Application.Errors;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Attendences
{
    public class Delete
    {
        public class Command : IRequest
        {
            public Command(int id)
            {
                Id = id;
            }

            public int Id { get; private set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = _context.Activities.GetAllData()
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                
                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound, new {Activity = "Not found"});

                var user = _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetCurrentUsername(),
                    cancellationToken);

                var attendance = await _context.ActivityAttendees.FirstOrDefaultAsync(
                    x => x.ActivityId == activity.Id && x.AppUserId == user.Id, cancellationToken);

                if (attendance != null)
                {
                    _context.ActivityAttendees.Remove(attendance);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return Unit.Value;
            }
        }
    }
}