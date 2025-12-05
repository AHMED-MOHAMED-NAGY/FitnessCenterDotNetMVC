using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fitnessCenter.Migrations
{
    /// <inheritdoc />
    public partial class init1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dailyGoals_men_UserId",
                table: "dailyGoals");

            migrationBuilder.DropForeignKey(
                name: "FK_men_exercises_exerciseexId",
                table: "men");

            migrationBuilder.DropForeignKey(
                name: "FK_men_men_CotchmanId",
                table: "men");

            migrationBuilder.DropIndex(
                name: "IX_men_CotchmanId",
                table: "men");

            migrationBuilder.DropIndex(
                name: "IX_men_exerciseexId",
                table: "men");

            migrationBuilder.DropColumn(
                name: "CotchmanId",
                table: "men");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "men");

            migrationBuilder.DropColumn(
                name: "available_times",
                table: "men");

            migrationBuilder.DropColumn(
                name: "cotch_status",
                table: "men");

            migrationBuilder.DropColumn(
                name: "exerciseexId",
                table: "men");

            migrationBuilder.DropColumn(
                name: "subscribeStatus",
                table: "men");

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    manId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.manId);
                    table.ForeignKey(
                        name: "FK_Admins_men_manId",
                        column: x => x.manId,
                        principalTable: "men",
                        principalColumn: "manId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cotches",
                columns: table => new
                {
                    manId = table.Column<int>(type: "integer", nullable: false),
                    cotch_status = table.Column<string>(type: "text", nullable: false),
                    available_times = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cotches", x => x.manId);
                    table.ForeignKey(
                        name: "FK_cotches_men_manId",
                        column: x => x.manId,
                        principalTable: "men",
                        principalColumn: "manId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    manId = table.Column<int>(type: "integer", nullable: false),
                    subscribeStatus = table.Column<string>(type: "text", nullable: true),
                    exerciseexId = table.Column<int>(type: "integer", nullable: true),
                    CotchmanId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.manId);
                    table.ForeignKey(
                        name: "FK_Users_cotches_CotchmanId",
                        column: x => x.CotchmanId,
                        principalTable: "cotches",
                        principalColumn: "manId");
                    table.ForeignKey(
                        name: "FK_Users_exercises_exerciseexId",
                        column: x => x.exerciseexId,
                        principalTable: "exercises",
                        principalColumn: "exId");
                    table.ForeignKey(
                        name: "FK_Users_men_manId",
                        column: x => x.manId,
                        principalTable: "men",
                        principalColumn: "manId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_CotchmanId",
                table: "Users",
                column: "CotchmanId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_exerciseexId",
                table: "Users",
                column: "exerciseexId");

            migrationBuilder.AddForeignKey(
                name: "FK_dailyGoals_Users_UserId",
                table: "dailyGoals",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "manId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dailyGoals_Users_UserId",
                table: "dailyGoals");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "cotches");

            migrationBuilder.AddColumn<int>(
                name: "CotchmanId",
                table: "men",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "men",
                type: "character varying(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<List<string>>(
                name: "available_times",
                table: "men",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cotch_status",
                table: "men",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "exerciseexId",
                table: "men",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "subscribeStatus",
                table: "men",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_men_CotchmanId",
                table: "men",
                column: "CotchmanId");

            migrationBuilder.CreateIndex(
                name: "IX_men_exerciseexId",
                table: "men",
                column: "exerciseexId");

            migrationBuilder.AddForeignKey(
                name: "FK_dailyGoals_men_UserId",
                table: "dailyGoals",
                column: "UserId",
                principalTable: "men",
                principalColumn: "manId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_men_exercises_exerciseexId",
                table: "men",
                column: "exerciseexId",
                principalTable: "exercises",
                principalColumn: "exId");

            migrationBuilder.AddForeignKey(
                name: "FK_men_men_CotchmanId",
                table: "men",
                column: "CotchmanId",
                principalTable: "men",
                principalColumn: "manId");
        }
    }
}
