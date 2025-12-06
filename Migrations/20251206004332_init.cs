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
                name: "dailyGoals",
                columns: table => new
                {
                    goalId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    status = table.Column<bool>(type: "boolean", nullable: false),
                    goal = table.Column<string>(type: "text", nullable: true),
                    date = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dailyGoals", x => x.goalId);
                });

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
                name: "men",
                columns: table => new
                {
                    manId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    userName = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    passwordHash = table.Column<string>(type: "text", nullable: false),
                    boy = table.Column<int>(type: "integer", nullable: false),
                    wight = table.Column<int>(type: "integer", nullable: false),
                    age = table.Column<int>(type: "integer", nullable: false),
                    number = table.Column<long>(type: "bigint", nullable: false),
                    whoIam = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_men", x => x.manId);
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
                name: "Cotches",
                columns: table => new
                {
                    manId = table.Column<int>(type: "integer", nullable: false),
                    cotch_status = table.Column<string>(type: "text", nullable: false),
                    available_times = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotches", x => x.manId);
                    table.ForeignKey(
                        name: "FK_Cotches_men_manId",
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
                    dailyGoalgoalId = table.Column<int>(type: "integer", nullable: true),
                    subscribeStatus = table.Column<string>(type: "text", nullable: true),
                    exerciseexId = table.Column<int>(type: "integer", nullable: true),
                    CotchmanId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.manId);
                    table.ForeignKey(
                        name: "FK_Users_Cotches_CotchmanId",
                        column: x => x.CotchmanId,
                        principalTable: "Cotches",
                        principalColumn: "manId");
                    table.ForeignKey(
                        name: "FK_Users_dailyGoals_dailyGoalgoalId",
                        column: x => x.dailyGoalgoalId,
                        principalTable: "dailyGoals",
                        principalColumn: "goalId");
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
                name: "IX_Users_dailyGoalgoalId",
                table: "Users",
                column: "dailyGoalgoalId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_exerciseexId",
                table: "Users",
                column: "exerciseexId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Cotches");

            migrationBuilder.DropTable(
                name: "dailyGoals");

            migrationBuilder.DropTable(
                name: "exercises");

            migrationBuilder.DropTable(
                name: "men");
        }
    }
}
