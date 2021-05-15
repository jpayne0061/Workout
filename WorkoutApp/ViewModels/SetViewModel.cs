using WorkoutApp.Enums;
using WorkoutApp.Models;

namespace WorkoutApp.ViewModels
{
    public class SetViewModel
    {
        public SetViewModel()
        {

        }

        public SetViewModel(WorkoutSetResult previousWorkoutSetResult)
        {
            if(previousWorkoutSetResult == null)
            {
                return;
            }

            Weight = previousWorkoutSetResult.Weight == null ? 0 : (int)previousWorkoutSetResult.Weight;
            RepsCompleted = previousWorkoutSetResult.RepsCompleted == null ? 0 : (int)previousWorkoutSetResult.RepsCompleted;
            Difficulty = previousWorkoutSetResult.Difficulty == null ? 0 : (Difficulty)previousWorkoutSetResult.Difficulty;
            Notes = previousWorkoutSetResult.Notes;
        }

        public Exercise Exercise { get; set; }
        public WorkoutSet WorkoutSet { get; set; }
        public int RepsCompleted { get; set; }
        public int Weight { get; set; }
        public Difficulty Difficulty { get; set; }
        public string Notes { get; set; }
        public bool IsLastSet { get; set; }
        public int WorkoutId { get; set; }
        public int WorkoutSessionId { get; set; }
        public WorkoutSetResult PreviousWorkoutSetResult { get; set; }
        
    }
}
