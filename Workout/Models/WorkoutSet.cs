using System;
using System.Collections.Generic;

namespace Workout.Models
{
    public partial class WorkoutSet
    {
        public WorkoutSet()
        {
            WorkoutSetResult = new HashSet<WorkoutSetResult>();
        }

        public int WorkoutId { get; set; }
        public int ExerciseId { get; set; }
        public DateTime DateCreated { get; set; }
        public int Reps { get; set; }
        public int SetOrder { get; set; }
        public int WorkoutSetId { get; set; }
        public bool Active { get; set; }

        public Exercise Exercise { get; set; }
        public Workout Workout { get; set; }
        public ICollection<WorkoutSetResult> WorkoutSetResult { get; set; }
    }
}
