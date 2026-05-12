using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Todo.Infrastructure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskShares : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "shared_with_user_ids",
                table: "todo_tasks");

            migrationBuilder.CreateTable(
                name: "task_shares",
                columns: table => new
                {
                    task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    shared_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_shares", x => new { x.task_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_task_shares_todo_tasks_task_id",
                        column: x => x.task_id,
                        principalTable: "todo_tasks",
                        principalColumn: "TaskId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_task_shares_users_shared_by_user_id",
                        column: x => x.shared_by_user_id,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_task_shares_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_task_shares_shared_by_user_id",
                table: "task_shares",
                column: "shared_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_task_shares_task_id",
                table: "task_shares",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "IX_task_shares_user_id",
                table: "task_shares",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "task_shares");

            migrationBuilder.AddColumn<string>(
                name: "shared_with_user_ids",
                table: "todo_tasks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
