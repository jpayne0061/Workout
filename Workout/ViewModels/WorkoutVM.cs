using System.Collections.Generic;
using Workout.Models;
using System.Linq;

namespace Workout.ViewModels
{
    public class WorkoutVM
    {
        public WorkoutVM()
        {
            WorkoutSets = new List<WorkoutSet>();
        }

        public WorkoutVM(Workout.Models.Workout workout)
        {
            Workout = workout;
            AllExercises = workout.WorkoutToExercise.Select(ws => new ExerciseVM(ws.Exercise.ExerciseName)).ToList();
        }

        public Workout.Models.Workout Workout { get; set; }
        public List<ExerciseVM> AllExercises { get; set; }
        public List<WorkoutSet> WorkoutSets { get; set; }
        public Dictionary<string, List<string>> ExerciseDisplay { get; set; }
    }
}
