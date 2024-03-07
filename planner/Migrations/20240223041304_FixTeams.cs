using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace planner.Migrations
{
    /// <inheritdoc />
    public partial class FixTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Teams");

            migrationBuilder.AddColumn<string>(
                name: "OwnerEmail",
                table: "Teams",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerEmail",
                table: "Teams");

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Teams",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
