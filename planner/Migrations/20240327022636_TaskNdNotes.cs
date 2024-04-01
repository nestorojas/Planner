using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace planner.Migrations
{
    /// <inheritdoc />
    public partial class TaskNdNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "Tasks");

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Notes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Notes");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Tasks",
                type: "TEXT",
                nullable: true);
        }
    }
}
