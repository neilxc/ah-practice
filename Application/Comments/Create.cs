using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using AutoMapper;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Comments
{
    public class Create
    {
        public class CommentData
        {
            public string Body { get; set; }
        }

        public class CommentDataValidator : AbstractValidator<CommentData>
        {
            public CommentDataValidator()
            {
                RuleFor(x => x.Body).NotNull().NotEmpty();
            }
        }

        public class Command : IRequest<CommentDto>
        {
            public CommentData Comment { get; set; }
            public int ActivityId { get; set; }
        }

        public class Handler : IRequestHandler<Command, CommentDto>
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
            
            public Task<CommentDto> Handle(Command request, CancellationToken cancellationToken)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}