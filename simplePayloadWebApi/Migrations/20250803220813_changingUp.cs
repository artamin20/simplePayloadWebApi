using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace simplePayloadWebApi.Migrations
{
    /// <inheritdoc />
    public partial class changingUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Event",
                table: "UserInteraction");

            migrationBuilder.RenameColumn(
                name: "IP",
                table: "UserInteraction",
                newName: "UserAgent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserAgent",
                table: "UserInteraction",
                newName: "IP");

            migrationBuilder.AddColumn<string>(
                name: "Event",
                table: "UserInteraction",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
