using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyTeams.Data.Migrations
{
    /// <inheritdoc />
    public partial class LeaveRequestandAuthorisationDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AuthorisedDate",
                table: "Leaves",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestDate",
                table: "Leaves",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorisedDate",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "RequestDate",
                table: "Leaves");
        }
    }
}
