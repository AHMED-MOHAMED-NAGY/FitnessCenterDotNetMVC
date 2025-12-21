using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fitnessCenter.Migrations
{
    /// <inheritdoc />
    public partial class FixDailyGoalSchema_Nullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_dailyGoals_dailyGoalgoalId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_dailyGoalgoalId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "dailyGoalgoalId",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "dailyGoals",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_dailyGoals_UserId",
                table: "dailyGoals",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_dailyGoals_Users_UserId",
                table: "dailyGoals",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "manId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dailyGoals_Users_UserId",
                table: "dailyGoals");

            migrationBuilder.DropIndex(
                name: "IX_dailyGoals_UserId",
                table: "dailyGoals");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "dailyGoals");

            migrationBuilder.AddColumn<int>(
                name: "dailyGoalgoalId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_dailyGoalgoalId",
                table: "Users",
                column: "dailyGoalgoalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_dailyGoals_dailyGoalgoalId",
                table: "Users",
                column: "dailyGoalgoalId",
                principalTable: "dailyGoals",
                principalColumn: "goalId");
        }
    }
}
