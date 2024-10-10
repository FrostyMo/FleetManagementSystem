using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetManagementSystem.Migrations
{
    public partial class AddInsurance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FuelCards_DriverId",
                table: "FuelCards");

            migrationBuilder.AddColumn<int>(
                name: "InsuranceId",
                table: "Vehicles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InsuranceId",
                table: "Drivers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Insurances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PolicyNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Company = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    CoverageAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Premium = table.Column<decimal>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PolicyDocuments = table.Column<string>(type: "TEXT", nullable: true),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Insurances", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_InsuranceId",
                table: "Vehicles",
                column: "InsuranceId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelCards_DriverId",
                table: "FuelCards",
                column: "DriverId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_InsuranceId",
                table: "Drivers",
                column: "InsuranceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_Insurances_InsuranceId",
                table: "Drivers",
                column: "InsuranceId",
                principalTable: "Insurances",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Insurances_InsuranceId",
                table: "Vehicles",
                column: "InsuranceId",
                principalTable: "Insurances",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_Insurances_InsuranceId",
                table: "Drivers");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Insurances_InsuranceId",
                table: "Vehicles");

            migrationBuilder.DropTable(
                name: "Insurances");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_InsuranceId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_FuelCards_DriverId",
                table: "FuelCards");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_InsuranceId",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "InsuranceId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "InsuranceId",
                table: "Drivers");

            migrationBuilder.CreateIndex(
                name: "IX_FuelCards_DriverId",
                table: "FuelCards",
                column: "DriverId");
        }
    }
}
