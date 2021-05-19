using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Workout.Models
{
    public class InProgressSession
    {
        public int WorkoutId { get; set; }
        public int WorkoutSetId { get; set; }
        public bool LastSet { get; set; }
        public int CurrentExerciseId { get; set; }
        public int WorkoutSessionId { get; set; }
    }
}
