using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jobportalwebsite.Migrations
{
    /// <inheritdoc />
    public partial class JobseekersIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Jobseekers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Jobseekers");
        }
    }
}
