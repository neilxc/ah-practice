using System.Collections.Generic;
using Domain;

namespace Application.Activities
{
    public class ActivitiesEnvelope
    {
        public List<ActivityDTO> Activities { get; set; }
        public int ActivityCount { get; set; }
    }
}