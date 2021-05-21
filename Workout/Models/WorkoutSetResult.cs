using System;
using System.Collections.Generic;

namespace Workout.Models
{
    public partial class WorkoutSetResult
    {
        public int WorkoutSetId { get; set; }
        public DateTime DateCreated { get; set; }
        public int? RepsCompleted { get; set; }
        public int? Difficulty { get; set; }
        public string Notes { get; set; }
        public int? Weight { get; set; }
        public int WorkoutSetResultId { get; set; }
        public int WorkoutSessionId { get; set; }
        public WorkoutSet WorkoutSet { get; set; }
        public bool Skipped { get; set; }
    }
}
