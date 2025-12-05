using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fitnessCenter.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cotches_men_manId",
                table: "cotches");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_cotches_CotchmanId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cotches",
                table: "cotches");

            migrationBuilder.RenameTable(
                name: "cotches",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                newName: "cotches");

            migrationBuilder.AddPrimaryKey(
                name: "PK_cotches",
                table: "cotches",
                column: "manId");

            migrationBuilder.AddForeignKey(
                name: "FK_cotches_men_manId",
                table: "cotches",
                column: "manId",
                principalTable: "men",
                principalColumn: "manId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_cotches_CotchmanId",
                table: "Users",
                column: "CotchmanId",
                principalTable: "cotches",
                principalColumn: "manId");
        }
    }
}
