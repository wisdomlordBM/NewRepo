using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jobportalwebsite.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingJobSeekerColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "FieldOfStudy",
                table: "Jobseekers");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "Jobseekers");

            migrationBuilder.DropColumn(
                name: "SchoolName",
                table: "Jobseekers");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Jobseekers");
        }
    }
}
