using Microsoft.EntityFrameworkCore.Migrations;

namespace Workout.Data.Migrations
{
    public partial class AddAspNetUserIdToWorkout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AspNetUserId",
                table: "Workout",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AspNetUserId",
                table: "Workout");
        }
    }
}
