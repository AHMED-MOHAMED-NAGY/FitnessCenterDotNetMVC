using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fitnessCenter.Migrations
{
    /// <inheritdoc />
    public partial class AddCoachSpecialty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExerciseId",
                table: "Cotches",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cotches_ExerciseId",
                table: "Cotches",
                column: "ExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cotches_exercises_ExerciseId",
                table: "Cotches",
                column: "ExerciseId",
                principalTable: "exercises",
                principalColumn: "exId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cotches_exercises_ExerciseId",
                table: "Cotches");

            migrationBuilder.DropIndex(
                name: "IX_Cotches_ExerciseId",
                table: "Cotches");

            migrationBuilder.DropColumn(
                name: "ExerciseId",
                table: "Cotches");
        }
    }
}
