using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jobportalwebsite.Migrations
{
    /// <inheritdoc />
    public partial class AddCVPathToApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CVPath",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CVPath",
                table: "Applications");
        }
    }
}
