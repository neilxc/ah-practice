using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using AutoMapper;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class ProfileReader : IProfileReader
    {
        private readonly DataContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly IMapper _mapper;

        public ProfileReader(DataContext context, IUserAccessor userAccessor, IMapper mapper)
        {
            _context = context;
            _userAccessor = userAccessor;
            _mapper = mapper;
        }
        
        public async Task<Profile> ReadProfile(string username)
        {
            var currentUserName = _userAccessor.GetCurrentUsername();

            var user = await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserName == username);
            
            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, new {User = "Not found"});

            var profile = _mapper.Map<AppUser, Profile>(user);

            if (currentUserName != null)
            {
                var currentUser = await _context.Users
                    .Include(x => x.Following)
                    .Include(x => x.Followers)
                    .FirstOrDefaultAsync(x => x.UserName == currentUserName);

                if (currentUser.Followers.Any(x => x.TargetId == user.Id))
                {
                    profile.IsFollowed = true;
                }
            }

            return profile;
        }
    }
}