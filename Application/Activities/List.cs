using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Persistence;

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<ActivitiesEnvelope>
        {
            public Query(string sort, string username, bool host, int? limit, int? offset)
            {
                Sort = sort;
                Username = username;
                Host = host;
                Limit = limit;
                Offset = offset;
            }

            public string Sort { get; }
            public string Username { get; }
            public bool Host { get; }
            public int? Limit { get; }
            public int? Offset { get; }
        }

        public class Handler : IRequestHandler<Query, ActivitiesEnvelope>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<ActivitiesEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var queryable = _context.Activities.GetAllData();
                
                if (!string.IsNullOrEmpty(request.Username))
                {
                    queryable = queryable
                        .Where(a => a.Attendees.Any(x => x.AppUser.UserName == request.Username));
                }

                if (!string.IsNullOrEmpty(request.Username) && request.Host)
                {
                    queryable = queryable
                        .Where(a => a.Attendees.Any(x => x.IsHost));
                }

                if (!string.IsNullOrEmpty(request.Sort))
                {
                    switch (request.Sort)
                    {
                        case "past":
                            queryable = queryable
                                .Where(x => x.Date < DateTime.Now)
                                .OrderByDescending(x => x.Date);
                            break;
                        default:
                            queryable = queryable
                                .Where(x => x.Date > DateTime.Now)
                                .OrderBy(x => x.Date);
                            break;
                    } 
                }

                var activitiesFromDb = await queryable
                    .Skip(request.Offset ?? 0)
                    .Take(request.Limit ?? 3)
                    .ToListAsync(cancellationToken);

                var activities = _mapper.Map<List<Activity>, List<ActivityDTO>>(activitiesFromDb);

                return new ActivitiesEnvelope
                {
                    Activities = activities,
                    ActivityCount = queryable.Count()
                };
            }
        }
    }
}