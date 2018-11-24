using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Delete
    {
        public class Command : IRequest
        {
            public Command(int id)
            {
                Id = id;
            }

            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            private readonly ICloudinaryAccessor _cloudinaryAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor, ICloudinaryAccessor cloudinaryAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
                _cloudinaryAccessor = cloudinaryAccessor;
            }
            
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.Include(p => p.Photos)
                    .FirstOrDefaultAsync(u => u.UserName == _userAccessor.GetCurrentUsername(), cancellationToken);
                
                if (user.Photos.All(p => p.Id != request.Id))
                    throw new RestException(HttpStatusCode.Unauthorized);

                var photoFromDb = await _context.Photos.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
                
                if (photoFromDb.IsMain)
                    throw new RestException(HttpStatusCode.BadRequest, new {Photo = "Cannot delete main photo"});

                _cloudinaryAccessor.DeletePhotoForUser(photoFromDb);

                return Unit.Value;
            }
        }
    }  
}