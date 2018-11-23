using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<ActivitiesEnvelope>{}

        public class Handler : IRequestHandler<Query, ActivitiesEnvelope>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
            
            public async Task<ActivitiesEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var queryable = _context.Activities.OrderByDescending(x => x.Date);

                var activities = await queryable.ToListAsync(cancellationToken);

                return new ActivitiesEnvelope
                {
                    Activities = activities,
                    ActivityCount = queryable.Count()
                };
            }
        }
    }
}