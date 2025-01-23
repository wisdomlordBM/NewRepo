using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jobportalwebsite.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Replies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "Replies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JobseekerId",
                table: "Replies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JobseekerId",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Replies_CompanyId",
                table: "Replies",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Replies_JobseekerId",
                table: "Replies",
                column: "JobseekerId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CompanyId",
                table: "Comments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_JobseekerId",
                table: "Comments",
                column: "JobseekerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Companies_CompanyId",
                table: "Comments",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_User_JobseekerId",
                table: "Comments",
                column: "JobseekerId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Replies_Companies_CompanyId",
                table: "Replies",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Replies_User_JobseekerId",
                table: "Replies",
                column: "JobseekerId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Companies_CompanyId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_User_JobseekerId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Replies_Companies_CompanyId",
                table: "Replies");

            migrationBuilder.DropForeignKey(
                name: "FK_Replies_User_JobseekerId",
                table: "Replies");

            migrationBuilder.DropIndex(
                name: "IX_Replies_CompanyId",
                table: "Replies");

            migrationBuilder.DropIndex(
                name: "IX_Replies_JobseekerId",
                table: "Replies");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CompanyId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_JobseekerId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Replies");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "Replies");

            migrationBuilder.DropColumn(
                name: "JobseekerId",
                table: "Replies");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "JobseekerId",
                table: "Comments");
        }
    }
}
