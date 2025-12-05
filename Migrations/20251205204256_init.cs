using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace fitnessCenter.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "exercises",
                columns: table => new
                {
                    exId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    exerciseType = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercises", x => x.exId);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    notId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "text", nullable: false),
                    msj = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.notId);
                });

            migrationBuilder.CreateTable(
                name: "men",
                columns: table => new
                {
                    manId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    userName = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    boy = table.Column<int>(type: "integer", nullable: false),
                    wight = table.Column<int>(type: "integer", nullable: false),
                    age = table.Column<int>(type: "integer", nullable: false),
                    whoIam = table.Column<string>(type: "text", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    cotch_status = table.Column<string>(type: "text", nullable: true),
                    available_times = table.Column<List<string>>(type: "text[]", nullable: true),
                    subscribeStatus = table.Column<string>(type: "text", nullable: true),
                    exerciseexId = table.Column<int>(type: "integer", nullable: true),
                    CotchmanId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_men", x => x.manId);
                    table.ForeignKey(
                        name: "FK_men_exercises_exerciseexId",
                        column: x => x.exerciseexId,
                        principalTable: "exercises",
                        principalColumn: "exId");
                    table.ForeignKey(
                        name: "FK_men_men_CotchmanId",
                        column: x => x.CotchmanId,
                        principalTable: "men",
                        principalColumn: "manId");
                });

            migrationBuilder.CreateTable(
                name: "dailyGoals",
                columns: table => new
                {
                    goalId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    status = table.Column<bool>(type: "boolean", nullable: false),
                    goal = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dailyGoals", x => x.goalId);
                    table.ForeignKey(
                        name: "FK_dailyGoals_men_UserId",
                        column: x => x.UserId,
                        principalTable: "men",
                        principalColumn: "manId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_dailyGoals_UserId",
                table: "dailyGoals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_men_CotchmanId",
                table: "men",
                column: "CotchmanId");

            migrationBuilder.CreateIndex(
                name: "IX_men_exerciseexId",
                table: "men",
                column: "exerciseexId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dailyGoals");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "men");

            migrationBuilder.DropTable(
                name: "exercises");
        }
    }
}
