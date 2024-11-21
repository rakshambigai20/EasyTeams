using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyTeams.Data.Migrations
{
    /// <inheritdoc />
    public partial class EasyTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Rejected",
                table: "Leaves",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rejected",
                table: "Leaves");
        }
    }
}
