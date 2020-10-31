using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Repository.Migrations
{
    public partial class InitialDbModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blobs",
                columns: table => new
                {
                    Key = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    MimeType = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blobs", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Key = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "PlannerTasks",
                columns: table => new
                {
                    Key = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Priority = table.Column<char>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    StatusDetail = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlannerTasks", x => x.Key);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blobs_Date",
                table: "Blobs",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_Date",
                table: "Notes",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_PlannerTasks_Date",
                table: "PlannerTasks",
                column: "Date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blobs");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "PlannerTasks");
        }
    }
}
