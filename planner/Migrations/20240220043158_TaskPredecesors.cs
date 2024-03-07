using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace planner.Migrations
{
    /// <inheritdoc />
    public partial class TaskPredecesors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Tasks_PredecessorId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_PredecessorId",
                table: "Tasks");

            migrationBuilder.AlterColumn<string>(
                name: "PredecessorId",
                table: "Tasks",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PredecessorId",
                table: "Tasks",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_PredecessorId",
                table: "Tasks",
                column: "PredecessorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Tasks_PredecessorId",
                table: "Tasks",
                column: "PredecessorId",
                principalTable: "Tasks",
                principalColumn: "Id");
        }
    }
}
