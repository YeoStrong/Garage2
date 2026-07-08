using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage2.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedSpotNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedSpotNumber",
                table: "ParkedVehicle",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParkedVehicle_RegistrationNumber",
                table: "ParkedVehicle",
                column: "RegistrationNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ParkedVehicle_RegistrationNumber",
                table: "ParkedVehicle");

            migrationBuilder.DropColumn(
                name: "AssignedSpotNumber",
                table: "ParkedVehicle");
        }
    }
}
