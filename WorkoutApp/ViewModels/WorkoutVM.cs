using System.Collections.Generic;
using WorkoutApp.Models;
using System.Linq;

namespace WorkoutApp.ViewModels
{
    public class WorkoutVM
    {
        public WorkoutVM()
        {
            WorkoutSets = new List<WorkoutSet>();
        }

        public WorkoutVM(Workout workout)
        {
            Workout = workout;
            AllExercises = workout.WorkoutSet.Select(ws => new ExerciseVM(ws.Exercise.ExerciseName)).ToList();
        }

        public Workout Workout { get; set; }
        public List<ExerciseVM> AllExercises { get; set; }
        public List<WorkoutSet> WorkoutSets { get; set; }
        public List<string> ExerciseDisplay { get; set; }
    }
}
