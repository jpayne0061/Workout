using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Workout.Models;

namespace Workout.ViewModels
{
    public class WorkoutHistoryVM
    {
        public Dictionary<string, List<WorkoutSetResult>> ExerciseHistories { get; set; }
    }
}
