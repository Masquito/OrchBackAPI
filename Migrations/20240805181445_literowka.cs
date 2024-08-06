using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orch_back_API.Migrations
{
    /// <inheritdoc />
    public partial class literowka : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DevlieryId",
                table: "Messages",
                newName: "DeliveryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeliveryId",
                table: "Messages",
                newName: "DevlieryId");
        }
    }
}
