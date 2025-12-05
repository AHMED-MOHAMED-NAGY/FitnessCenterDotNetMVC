using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fitnessCenter.Migrations
{
    /// <inheritdoc />
    public partial class init4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_dailyGoals_UserId",
                table: "dailyGoals");

            migrationBuilder.AddColumn<string>(
                name: "date",
                table: "dailyGoals",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_dailyGoals_UserId",
                table: "dailyGoals",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_dailyGoals_UserId",
                table: "dailyGoals");

            migrationBuilder.DropColumn(
                name: "date",
                table: "dailyGoals");

            migrationBuilder.CreateIndex(
                name: "IX_dailyGoals_UserId",
                table: "dailyGoals",
                column: "UserId");
        }
    }
}
