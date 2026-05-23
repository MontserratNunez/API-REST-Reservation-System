using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeGuestIdToReservationIdToReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdGuest",
                table: "Review");

            migrationBuilder.AddColumn<int>(
                name: "IdReservation",
                table: "Review",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdReservation",
                table: "Review");

            migrationBuilder.AddColumn<string>(
                name: "IdGuest",
                table: "Review",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
