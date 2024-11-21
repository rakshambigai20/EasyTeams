using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyTeams.Data.Migrations
{
    /// <inheritdoc />
    public partial class initialise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReportSubmitted",
                table: "Tasks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportSubmitted",
                table: "Tasks");
        }
    }
}
