using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Workout.Models;

namespace Workout.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Exercise> Exercise { get; set; }
        public virtual DbSet<Workout.Models.Workout> Workout { get; set; }
        public virtual DbSet<WorkoutSet> WorkoutSet { get; set; }
        public virtual DbSet<WorkoutSetResult> WorkoutSetResult { get; set; }
        public virtual DbSet<WorkoutToExercise> WorkoutToExercise { get; set; }
        public virtual DbSet<WorkoutSession> WorkoutSession { get; set; }

    }
}
