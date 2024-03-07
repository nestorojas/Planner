using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace planner.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTaskfromWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Projects_ProjectId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Workflows_WorkflowId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_WorkflowId",
                table: "Tasks");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Projects_ProjectId",
                table: "Tasks",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Projects_ProjectId",
                table: "Tasks");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_WorkflowId",
                table: "Tasks",
                column: "WorkflowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Projects_ProjectId",
                table: "Tasks",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Workflows_WorkflowId",
                table: "Tasks",
                column: "WorkflowId",
                principalTable: "Workflows",
                principalColumn: "Id");
        }
    }
}
