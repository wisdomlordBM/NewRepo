using Microsoft.EntityFrameworkCore.Migrations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

#nullable disable

namespace Jobportalwebsite.Migrations
{
    /// <inheritdoc />
    public partial class addJobseekerIdToApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_User_UserId1",
                table: "Applications");

            migrationBuilder.RenameColumn(
                name: "UserId1",
                table: "Applications",
                newName: "JobseekerId");

            migrationBuilder.RenameIndex(
                name: "IX_Applications_UserId1",
                table: "Applications",
                newName: "IX_Applications_JobseekerId");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "JobId",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Applications",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
            

            //migrationBuilder.AddColumn<int>(
            //    name: "JobSeekerId",
            //    table: "Applications",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_UserId",
                table: "Applications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Jobseekers_JobseekerId",
                table: "Applications",
                column: "JobseekerId",
                principalTable: "Jobseekers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_User_UserId",
                table: "Applications",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Jobseekers_JobseekerId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_User_UserId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_UserId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "JobSeekerId",
                table: "Applications");

            migrationBuilder.RenameColumn(
                name: "JobseekerId",
                table: "Applications",
                newName: "UserId1");

            migrationBuilder.RenameIndex(
                name: "IX_Applications_JobseekerId",
                table: "Applications",
                newName: "IX_Applications_UserId1");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_User_UserId1",
                table: "Applications",
                column: "UserId1",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
