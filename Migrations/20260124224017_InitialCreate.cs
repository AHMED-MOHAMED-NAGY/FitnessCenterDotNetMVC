using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fitnessCenter.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "exercises",
                columns: table => new
                {
                    exId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    exerciseType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercises", x => x.exId);
                });

            migrationBuilder.CreateTable(
                name: "men",
                columns: table => new
                {
                    manId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    userName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    passwordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    boy = table.Column<int>(type: "int", nullable: false),
                    wight = table.Column<int>(type: "int", nullable: false),
                    age = table.Column<int>(type: "int", nullable: false),
                    number = table.Column<long>(type: "bigint", nullable: false),
                    whoIam = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_men", x => x.manId);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    manId = table.Column<int>(type: "int", nullable: false)
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
                    manId = table.Column<int>(type: "int", nullable: false),
                    cotch_status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    available_times = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExerciseId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotches", x => x.manId);
                    table.ForeignKey(
                        name: "FK_Cotches_exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "exercises",
                        principalColumn: "exId");
                    table.ForeignKey(
                        name: "FK_Cotches_men_manId",
                        column: x => x.manId,
                        principalTable: "men",
                        principalColumn: "manId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    notId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    msj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ManId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.notId);
                    table.ForeignKey(
                        name: "FK_notifications_men_ManId",
                        column: x => x.ManId,
                        principalTable: "men",
                        principalColumn: "manId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    manId = table.Column<int>(type: "int", nullable: false),
                    CotchId = table.Column<int>(type: "int", nullable: true),
                    subscribeStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubscriptionPlan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubscriptionStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubscriptionEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    exerciseexId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.manId);
                    table.ForeignKey(
                        name: "FK_Users_Cotches_CotchId",
                        column: x => x.CotchId,
                        principalTable: "Cotches",
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

            migrationBuilder.CreateTable(
                name: "appointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CotchId = table.Column<int>(type: "int", nullable: false),
                    ExerciseId = table.Column<int>(type: "int", nullable: false),
                    AppointmentDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_appointments_Cotches_CotchId",
                        column: x => x.CotchId,
                        principalTable: "Cotches",
                        principalColumn: "manId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_appointments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "manId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_appointments_exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "exercises",
                        principalColumn: "exId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dailyGoals",
                columns: table => new
                {
                    goalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<bool>(type: "bit", nullable: false),
                    goal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dailyGoals", x => x.goalId);
                    table.ForeignKey(
                        name: "FK_dailyGoals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "manId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_appointments_CotchId",
                table: "appointments",
                column: "CotchId");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_ExerciseId",
                table: "appointments",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_UserId",
                table: "appointments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cotches_ExerciseId",
                table: "Cotches",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_dailyGoals_UserId",
                table: "dailyGoals",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_ManId",
                table: "notifications",
                column: "ManId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CotchId",
                table: "Users",
                column: "CotchId");

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
                name: "appointments");

            migrationBuilder.DropTable(
                name: "dailyGoals");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Cotches");

            migrationBuilder.DropTable(
                name: "exercises");

            migrationBuilder.DropTable(
                name: "men");
        }
    }
}
