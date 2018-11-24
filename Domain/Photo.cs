using System;

namespace Domain
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }

        public AppUser AppUser { get; set; }
        public int AppUserId { get; set; }
    }
}