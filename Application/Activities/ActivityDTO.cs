using System;
using System.Collections.Generic;
using Application.Attendences;
using Domain;

namespace Application.Activities
{
    public class ActivityDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string City { get; set; }
        public string Venue { get; set; }
        public GeoCoordinate GeoCoordinate { get; set; }
        public ICollection<AttendeeDTO> Attendees { get; set; }
    }
}