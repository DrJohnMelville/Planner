using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Repository.Migrations
{
    public partial class Appointments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppointmentDetails",
                columns: table => new
                {
                    AppointmentDetailsId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    BodyText = table.Column<string>(type: "TEXT", nullable: false),
                    UniqueOutlookId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentDetails", x => x.AppointmentDetailsId);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Start = table.Column<long>(type: "TEXT", nullable: false),
                    AppointmentDetailsId = table.Column<Guid>(type: "TEXT", nullable: false),
                    End = table.Column<long>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => new { x.AppointmentDetailsId, x.Start });
                    table.ForeignKey(
                        name: "FK_Appointments_AppointmentDetails_AppointmentDetailsId",
                        column: x => x.AppointmentDetailsId,
                        principalTable: "AppointmentDetails",
                        principalColumn: "AppointmentDetailsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDetails_UniqueOutlookId",
                table: "AppointmentDetails",
                column: "UniqueOutlookId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "AppointmentDetails");
        }
    }
}
