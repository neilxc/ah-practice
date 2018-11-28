using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
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

            public Handler(DataContext context)
            {
                _context = context;
            }
            
            public Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}