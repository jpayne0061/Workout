using System;
using System.Collections.Generic;

namespace WorkoutApp.Models
{
    public partial class AppUser
    {
        public AppUser()
        {
            Workout = new HashSet<Workout>();
        }

        public int AppUserId { get; set; }
        public DateTime DateCreated { get; set; }
        public string AppUsername { get; set; }

        public ICollection<Workout> Workout { get; set; }
    }
}
