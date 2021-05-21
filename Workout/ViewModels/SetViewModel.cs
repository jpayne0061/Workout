using Workout.Enums;
using Workout.Models;

namespace Workout.ViewModels
{
    public class SetViewModel
    {
        public SetViewModel()
        {

        }

        public SetViewModel(PreviousSetResult previousWorkoutSetResult)
        {
            if(previousWorkoutSetResult.WorkoutSetResult == null)
            {
                return;
            }

            PreviousWorkoutSetResult = previousWorkoutSetResult;

            Weight = previousWorkoutSetResult.WorkoutSetResult.Weight == null ? 0 : (int)previousWorkoutSetResult.WorkoutSetResult.Weight;
            RepsCompleted = previousWorkoutSetResult.WorkoutSetResult.RepsCompleted == null ? 0 : (int)previousWorkoutSetResult.WorkoutSetResult.RepsCompleted;
            Difficulty = previousWorkoutSetResult.WorkoutSetResult.Difficulty == null ? 0 : (Difficulty)previousWorkoutSetResult.WorkoutSetResult.Difficulty;

            switch(previousWorkoutSetResult.WorkoutSetResult.Difficulty){
                case 0:
                    PreviousDifficulty = "Easy";
                    break;
                case 1:
                    PreviousDifficulty = "Easy Medium";
                    break;
                case 2:
                    PreviousDifficulty = "Medium";
                    break;
                case 3:
                    PreviousDifficulty = "Medium Difficult";
                    break;
                case 4:
                    PreviousDifficulty = "Difficult";
                    break;
            }

        }

        public Exercise Exercise { get; set; }
        public WorkoutSet WorkoutSet { get; set; }
        public int RepsCompleted { get; set; }
        public int Weight { get; set; }
        public Difficulty Difficulty { get; set; }
        public string PreviousDifficulty { get; set; }
        public string Notes { get; set; }
        public bool IsLastSet { get; set; }
        public int WorkoutId { get; set; }
        public int WorkoutSessionId { get; set; }
        public PreviousSetResult PreviousWorkoutSetResult { get; set; }
    }
}
