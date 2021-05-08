using WorkoutApp.Enums;
using WorkoutApp.Models;

namespace WorkoutApp.ViewModels
{
    public class SetViewModel
    {
        public Exercise Exercise { get; set; }
        public WorkoutSet WorkoutSet { get; set; }
        public int RepsCompleted { get; set; }
        public int Weight { get; set; }
        public Difficulty Difficulty { get; set; }
        public string Notes { get; set; }
    }
}
