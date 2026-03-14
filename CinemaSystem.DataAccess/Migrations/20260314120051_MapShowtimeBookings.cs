using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MapShowtimeBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Showtimes_ShowtimeId",
                table: "Bookings");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Showtimes_ShowtimeId",
                table: "Bookings",
                column: "ShowtimeId",
                principalTable: "Showtimes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Showtimes_ShowtimeId",
                table: "Bookings");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Showtimes_ShowtimeId",
                table: "Bookings",
                column: "ShowtimeId",
                principalTable: "Showtimes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
