using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WorkoutApp.Models
{
    public partial class WorkoutContext : DbContext
    {
        public WorkoutContext()
        {
        }

        public WorkoutContext(DbContextOptions<WorkoutContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AppUser> AppUser { get; set; }
        public virtual DbSet<Exercise> Exercise { get; set; }
        public virtual DbSet<Workout> Workout { get; set; }
        public virtual DbSet<WorkoutSet> WorkoutSet { get; set; }
        public virtual DbSet<WorkoutSetResult> WorkoutSetResult { get; set; }
        public virtual DbSet<WorkoutToExercise> WorkoutToExercise { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.\\SQLExpress02;Database=Workout;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.Property(e => e.AppUserId).HasColumnName("AppUserID");

                entity.Property(e => e.AppUsername)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DateCreated).HasColumnType("datetime");
            });

            modelBuilder.Entity<Exercise>(entity =>
            {
                entity.Property(e => e.ExerciseId).HasColumnName("ExerciseID");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.ExerciseName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Workout>(entity =>
            {
                entity.Property(e => e.WorkoutId).HasColumnName("WorkoutID");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.WorkoutName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Workout)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Workout__Created__398D8EEE");
            });

            modelBuilder.Entity<WorkoutSet>(entity =>
            {
                entity.Property(e => e.WorkoutSetId).HasColumnName("WorkoutSetID");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.ExerciseId).HasColumnName("ExerciseID");

                entity.Property(e => e.WorkoutId).HasColumnName("WorkoutID");

                entity.HasOne(d => d.Exercise)
                    .WithMany(p => p.WorkoutSet)
                    .HasForeignKey(d => d.ExerciseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__WorkoutSe__Exerc__3F466844");

                entity.HasOne(d => d.Workout)
                    .WithMany(p => p.WorkoutSet)
                    .HasForeignKey(d => d.WorkoutId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__WorkoutSe__Worko__3E52440B");
            });

            modelBuilder.Entity<WorkoutSetResult>(entity =>
            {
                entity.Property(e => e.WorkoutSetResultId).HasColumnName("WorkoutSetResultID");

                entity.Property(e => e.Notes)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.WorkoutSetId).HasColumnName("WorkoutSetID");

                entity.HasOne(d => d.WorkoutSet)
                    .WithMany(p => p.WorkoutSetResult)
                    .HasForeignKey(d => d.WorkoutSetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__WorkoutSe__Worko__440B1D61");
            });

            modelBuilder.Entity<WorkoutToExercise>(entity =>
            {
                entity.Property(e => e.WorkoutToExerciseId).HasColumnName("WorkoutToExerciseID");

                entity.Property(e => e.ExerciseId).HasColumnName("ExerciseID");

                entity.Property(e => e.WorkoutId).HasColumnName("WorkoutID");

                entity.HasOne(d => d.Exercise)
                    .WithMany(p => p.WorkoutToExercise)
                    .HasForeignKey(d => d.ExerciseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__WorkoutTo__Exerc__45F365D3");

                entity.HasOne(d => d.Workout)
                    .WithMany(p => p.WorkoutToExercise)
                    .HasForeignKey(d => d.WorkoutId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__WorkoutTo__Worko__46E78A0C");
            });
        }
    }
}
