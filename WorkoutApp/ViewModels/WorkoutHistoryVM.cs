using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkoutApp.Models;

namespace WorkoutApp.ViewModels
{
    public class WorkoutHistoryVM
    {
        public Dictionary<string, List<WorkoutSetResult>> ExerciseHistories { get; set; }
    }
}
