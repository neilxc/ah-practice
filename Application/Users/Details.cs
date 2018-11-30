using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Users
{
    public abstract class Details
    {
        public class Query : IRequest<User>
        {
        }

        public class Handler : IRequestHandler<Query, User>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            private readonly IMapper _mapper;
            private readonly IJwtGenerator _jwtGenerator;

            public Handler(DataContext context, IUserAccessor userAccessor, IMapper mapper, IJwtGenerator jwtGenerator)
            {
                _context = context;
                _userAccessor = userAccessor;
                _mapper = mapper;
                _jwtGenerator = jwtGenerator;
            }

            public async Task<User> Handle(Query request, CancellationToken cancellationToken)
            {
                var userFromDb = await _context.Users.Include(p => p.Photos)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => 
                        x.UserName == _userAccessor.GetCurrentUsername(), cancellationToken);

                var user = _mapper.Map<AppUser, User>(userFromDb);
                user.Token = _jwtGenerator.CreateToken(userFromDb);
                
                return user;
            }
        }
    }
}