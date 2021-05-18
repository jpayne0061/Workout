using System;
using System.Collections.Generic;

namespace Workout.ViewModels
{
    public class ExerciseResultsHistory
    {
        public ExerciseResultsHistory()
        {
            Results = new List<string>();
        }
        public string ExerciseName { get; set; }
        public List<string> Results { get; set; }
    }

    public class WorkoutSessionVM
    {
        public DateTime WorkoutSessionDate { get; set; }
        public List<ExerciseResultsHistory> ExerciseHistories { get; set; }

    }
}
