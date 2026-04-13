using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddConcessionCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Concessions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Concessions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Concessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Concessions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Category", "ImageUrl" },
                values: new object[] { 0, null });

            migrationBuilder.UpdateData(
                table: "Concessions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Category", "ImageUrl" },
                values: new object[] { 3, null });

            migrationBuilder.UpdateData(
                table: "Concessions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Category", "ImageUrl" },
                values: new object[] { 1, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Concessions");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Concessions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Concessions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Concessions",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://via.placeholder.com/150/ffcc00/000000?text=Popcorn");

            migrationBuilder.UpdateData(
                table: "Concessions",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "https://via.placeholder.com/150/ff9900/000000?text=Nachos");

            migrationBuilder.UpdateData(
                table: "Concessions",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "https://via.placeholder.com/150/cc0000/ffffff?text=Cola");
        }
    }
}
