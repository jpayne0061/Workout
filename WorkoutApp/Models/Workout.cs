using System;
using System.Collections.Generic;

namespace WorkoutApp.Models
{
    public partial class Workout
    {
        public Workout()
        {
            WorkoutSet = new HashSet<WorkoutSet>();
            WorkoutToExercise = new HashSet<WorkoutToExercise>();
        }

        public int WorkoutId { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
        public string WorkoutName { get; set; }

        public AppUser CreatedByNavigation { get; set; }
        public ICollection<WorkoutSet> WorkoutSet { get; set; }
        public ICollection<WorkoutToExercise> WorkoutToExercise { get; set; }
    }
}
