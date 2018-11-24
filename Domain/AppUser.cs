using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class AppUser : IdentityUser<int>
    {
        public string Bio { get; set; }
        public ICollection<FollowedPeople> Following { get; set; }
        public ICollection<FollowedPeople> Followers { get; set; }
        public ICollection<ActivityAttendee> Attendees { get; set; }
    }
}