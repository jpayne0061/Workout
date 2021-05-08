using System.Collections.Generic;

namespace WorkoutApp.ViewModels
{
    public class ExerciseVM
    {
        public ExerciseVM()
        {

        }
        public ExerciseVM(string exerciseName)
        {
            ExerciseName = exerciseName;
        }

        public string ExerciseName { get; set; }
        public int ExerciseID { get; set; }
        public bool Selected { get; set; }
        public string SetsAndReps { get; set; }
        public int Order { get; set; }
    }
}
