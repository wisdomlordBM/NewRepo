using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jobportalwebsite.Migrations
{
    /// <inheritdoc />
    public partial class JobseekerUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Jobseekers_JobseekersId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_JobseekersId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Jobseekers");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "Jobseekers");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Jobseekers");

            migrationBuilder.DropColumn(
                name: "EducationLevel",
                table: "Jobseekers");

            migrationBuilder.DropColumn(
                name: "EmploymentType",
                table: "Jobseekers");

            migrationBuilder.DropColumn(
                name: "FieldOfStudy",
                table: "Jobseekers");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "Jobseekers");

            migrationBuilder.DropColumn(
                name: "Salary",
                table: "Jobseekers");

            migrationBuilder.DropColumn(
                name: "SchoolName",
                table: "Jobseekers");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Jobseekers");

            migrationBuilder.DropColumn(
                name: "JobseekersId",
                table: "Applications");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Jobseekers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Jobseekers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Jobseekers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Jobseekers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Contact",
                table: "Jobseekers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Jobseekers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jobseekers_UserId",
                table: "Jobseekers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobseekers_AspNetUsers_UserId",
                table: "Jobseekers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobseekers_AspNetUsers_UserId",
                table: "Jobseekers");

            migrationBuilder.DropIndex(
                name: "IX_Jobseekers_UserId",
                table: "Jobseekers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Jobseekers");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Jobseekers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Jobseekers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Jobseekers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Jobseekers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Contact",
                table: "Jobseekers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Jobseekers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "Jobseekers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Jobseekers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EducationLevel",
                table: "Jobseekers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmploymentType",
                table: "Jobseekers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FieldOfStudy",
                table: "Jobseekers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "Jobseekers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Salary",
                table: "Jobseekers",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SchoolName",
                table: "Jobseekers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Jobseekers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "JobseekersId",
                table: "Applications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_JobseekersId",
                table: "Applications",
                column: "JobseekersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Jobseekers_JobseekersId",
                table: "Applications",
                column: "JobseekersId",
                principalTable: "Jobseekers",
                principalColumn: "Id");
        }
    }
}
