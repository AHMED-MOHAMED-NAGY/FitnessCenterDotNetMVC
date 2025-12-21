using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fitnessCenter.Migrations
{
    /// <inheritdoc />
    public partial class AddManIdToNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ManId",
                table: "notifications",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_notifications_ManId",
                table: "notifications",
                column: "ManId");

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_men_ManId",
                table: "notifications",
                column: "ManId",
                principalTable: "men",
                principalColumn: "manId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notifications_men_ManId",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Cotches_CotchId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_notifications_ManId",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "ManId",
                table: "notifications");

            migrationBuilder.RenameColumn(
                name: "CotchId",
                table: "Users",
                newName: "CotchmanId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_CotchId",
                table: "Users",
                newName: "IX_Users_CotchmanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Cotches_CotchmanId",
                table: "Users",
                column: "CotchmanId",
                principalTable: "Cotches",
                principalColumn: "manId");
        }
    }
}
