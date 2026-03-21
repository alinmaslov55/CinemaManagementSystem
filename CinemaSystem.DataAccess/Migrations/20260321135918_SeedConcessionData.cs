using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CinemaSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedConcessionData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Concessions",
                columns: new[] { "Id", "CreatedDate", "Description", "ImageUrl", "IsActive", "IsDeleted", "Name", "Price", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Classic salted butter popcorn (150g)", "https://via.placeholder.com/150/ffcc00/000000?text=Popcorn", true, false, "Large Popcorn", 12.50m, null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Crispy tortilla chips with warm cheese dip", "https://via.placeholder.com/150/ff9900/000000?text=Nachos", true, false, "Cheese Nachos", 14.00m, null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "0.5L fountain drink", "https://via.placeholder.com/150/cc0000/ffffff?text=Cola", true, false, "Coca-Cola (Large)", 6.50m, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Concessions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Concessions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Concessions",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
