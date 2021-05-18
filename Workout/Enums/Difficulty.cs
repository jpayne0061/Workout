using System.ComponentModel;

namespace Workout.Enums
{
    public enum Difficulty
    {
        [Description("Easy")]
        Easy,
        [Description("EasyMedium")]
        EasyMedium,
        [Description("Medium")]
        Medium,
        [Description("MediumDifficult")]
        MediumDifficult,
        [Description("Difficult")]
        Difficult
    }
}
