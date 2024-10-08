using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetManagementSystem.Migrations
{
    public partial class AddFuel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FuelCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FuelCardNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Mileage = table.Column<int>(type: "INTEGER", nullable: false),
                    DriverId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FuelCards_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FuelCardDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FuelCardId = table.Column<int>(type: "INTEGER", nullable: false),
                    Usage = table.Column<decimal>(type: "TEXT", nullable: false),
                    MonthYear = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProofFilePath = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelCardDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FuelCardDetails_FuelCards_FuelCardId",
                        column: x => x.FuelCardId,
                        principalTable: "FuelCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FuelCardDetails_FuelCardId",
                table: "FuelCardDetails",
                column: "FuelCardId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelCards_DriverId",
                table: "FuelCards",
                column: "DriverId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FuelCardDetails");

            migrationBuilder.DropTable(
                name: "FuelCards");
        }
    }
}
