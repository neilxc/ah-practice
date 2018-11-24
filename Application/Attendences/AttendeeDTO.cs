using System;

namespace Application.Attendences
{
    public class AttendeeDTO
    {
        public string Username { get; set; }
        public DateTime DateJoined { get; set; }
        public string Image { get; set; }
        public bool IsHost { get; set; }
    }
}