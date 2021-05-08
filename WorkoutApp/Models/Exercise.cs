using System;
using System.Collections.Generic;

namespace WorkoutApp.Models
{
    public partial class Exercise
    {
        public Exercise()
        {
            WorkoutSet = new HashSet<WorkoutSet>();
            WorkoutToExercise = new HashSet<WorkoutToExercise>();
        }

        public int ExerciseId { get; set; }
        public DateTime DateCreated { get; set; }
        public string ExerciseName { get; set; }

        public ICollection<WorkoutSet> WorkoutSet { get; set; }
        public ICollection<WorkoutToExercise> WorkoutToExercise { get; set; }
    }
}
