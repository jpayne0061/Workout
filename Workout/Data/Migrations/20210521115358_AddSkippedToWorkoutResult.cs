using Microsoft.EntityFrameworkCore.Migrations;

namespace Workout.Data.Migrations
{
    public partial class AddSkippedToWorkoutResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Skipped",
                table: "WorkoutSetResult",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Skipped",
                table: "WorkoutSetResult");
        }
    }
}
