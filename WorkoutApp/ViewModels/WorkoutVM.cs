using System.Collections.Generic;
using WorkoutApp.Models;
using System.Linq;

namespace WorkoutApp.ViewModels
{
    //public class ExerciseDisplay
    //{
    //    public string ExerciseName { get; set; }
    //    public string SetsAndReps { get; set; }
    //}

    public class WorkoutVM
    {
        public WorkoutVM()
        {
            WorkoutSets = new List<WorkoutSet>();
        }

        public WorkoutVM(Workout workout)
        {
            Workout = workout;
            AllExercises = workout.WorkoutToExercise.Select(ws => new ExerciseVM(ws.Exercise.ExerciseName)).ToList();
        }

        public Workout Workout { get; set; }
        public List<ExerciseVM> AllExercises { get; set; }
        public List<WorkoutSet> WorkoutSets { get; set; }
        public Dictionary<string, List<string>> ExerciseDisplay { get; set; }
    }
}
