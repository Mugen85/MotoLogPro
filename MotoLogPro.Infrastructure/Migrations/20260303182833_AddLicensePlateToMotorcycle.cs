using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotoLogPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLicensePlateToMotorcycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LicensePlate",
                table: "Motorcycles",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicensePlate",
                table: "Motorcycles");
        }
    }
}
