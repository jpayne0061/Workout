using System.Collections.Generic;
using WorkoutApp.Models;

namespace WorkoutApp.ViewModels
{
    public class WorkoutVM
    {
        public WorkoutVM()
        {
            WorkoutSets = new List<WorkoutSet>();
        }

        public Workout Workout { get; set; }
        public List<ExerciseVM> AllExercises { get; set; }
        public List<WorkoutSet> WorkoutSets { get; set; }
    }
}
