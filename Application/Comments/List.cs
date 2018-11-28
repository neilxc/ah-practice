using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Persistence;

namespace Application.Comments
{
    public class List
    {
        public class Query : IRequest<List<CommentDto>>
        {
            public Query(int activityId)
            {
                ActivityId = activityId;
            }

            public int ActivityId { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<CommentDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public Task<List<CommentDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}