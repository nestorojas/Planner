using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace planner.Migrations
{
    /// <inheritdoc />
    public partial class TeamManager : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Clients",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTeamManager",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "IsTeamManager",
                table: "AspNetUsers");
        }
    }
}
