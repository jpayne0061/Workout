using System;
using System.Collections.Generic;

namespace WorkoutApp.Models
{
    public partial class WorkoutToExercise
    {
        public int ExerciseId { get; set; }
        public int WorkoutId { get; set; }
        public int WorkoutToExerciseId { get; set; }

        public Exercise Exercise { get; set; }
        public Workout Workout { get; set; }

        public int Order { get; set; }
    }
}
