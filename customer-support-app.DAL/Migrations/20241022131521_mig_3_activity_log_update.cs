using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace customer_support_app.DAL.Migrations
{
    /// <inheritdoc />
    public partial class mig_3_activity_log_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLogs_AspNetUsers_OwnerId",
                table: "ActivityLogs");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "ActivityLogs",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "ActivityType",
                table: "ActivityLogs",
                newName: "TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityLogs_OwnerId",
                table: "ActivityLogs",
                newName: "IX_ActivityLogs_UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_TicketId",
                table: "ActivityLogs",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLogs_AspNetUsers_UserId",
                table: "ActivityLogs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLogs_Tickets_TicketId",
                table: "ActivityLogs",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLogs_AspNetUsers_UserId",
                table: "ActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLogs_Tickets_TicketId",
                table: "ActivityLogs");

            migrationBuilder.DropIndex(
                name: "IX_ActivityLogs_TicketId",
                table: "ActivityLogs");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ActivityLogs",
                newName: "OwnerId");

            migrationBuilder.RenameColumn(
                name: "TicketId",
                table: "ActivityLogs",
                newName: "ActivityType");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityLogs_UserId",
                table: "ActivityLogs",
                newName: "IX_ActivityLogs_OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLogs_AspNetUsers_OwnerId",
                table: "ActivityLogs",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
