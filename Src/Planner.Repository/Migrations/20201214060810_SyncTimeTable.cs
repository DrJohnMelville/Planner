using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Repository.Migrations
{
    public partial class SyncTimeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SyncTimes",
                columns: table => new
                {
                    SyncTimeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Time = table.Column<long>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncTimes", x => x.SyncTimeId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SyncTimes");
        }
    }
}
