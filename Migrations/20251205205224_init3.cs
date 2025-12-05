using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fitnessCenter.Migrations
{
    /// <inheritdoc />
    public partial class init3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cotchs_men_manId",
                table: "Cotchs");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Cotchs_CotchmanId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cotchs",
                table: "Cotchs");

            migrationBuilder.RenameTable(
                name: "Cotchs",
                newName: "Cotches");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cotches",
                table: "Cotches",
                column: "manId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cotches_men_manId",
                table: "Cotches",
                column: "manId",
                principalTable: "men",
                principalColumn: "manId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Cotches_CotchmanId",
                table: "Users",
                column: "CotchmanId",
                principalTable: "Cotches",
                principalColumn: "manId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cotches_men_manId",
                table: "Cotches");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Cotches_CotchmanId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cotches",
                table: "Cotches");

            migrationBuilder.RenameTable(
                name: "Cotches",
                newName: "Cotchs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cotchs",
                table: "Cotchs",
                column: "manId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cotchs_men_manId",
                table: "Cotchs",
                column: "manId",
                principalTable: "men",
                principalColumn: "manId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Cotchs_CotchmanId",
                table: "Users",
                column: "CotchmanId",
                principalTable: "Cotchs",
                principalColumn: "manId");
        }
    }
}
