using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class Delete
    {
        public class Command : IRequest
        {
            public Command(int activityId, int commentId)
            {
                ActivityId = activityId;
                CommentId = commentId;
            }

            public int ActivityId { get; set; }
            public int CommentId { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.ActivityId).NotNull().NotEmpty();
            }
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
                var user = _context.Users
                    .FirstOrDefaultAsync(x =>x.UserName == _userAccessor.GetCurrentUsername(), 
                        cancellationToken);
                
                var activity = await _context.Activities
                    .Include(x => x.Comments)
                    .FirstOrDefaultAsync(x => x.Id == request.ActivityId, cancellationToken);
                
                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound, new {Activity = "Not found"});
              
                var comment = activity.Comments.FirstOrDefault(x => x.Id == request.CommentId);
                
                if (comment == null)
                    throw new RestException(HttpStatusCode.NotFound, new {Comment = "Not found"});
                
                if (comment.Author.Id != user.Id)
                {
                    throw new RestException(HttpStatusCode.Forbidden, new {Comment = "Can only delete own comment"});
                }

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}