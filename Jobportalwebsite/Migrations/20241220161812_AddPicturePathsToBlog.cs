using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jobportalwebsite.Migrations
{
    /// <inheritdoc />
    public partial class AddPicturePathsToBlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Picture",
                table: "Blogs");

            migrationBuilder.AddColumn<string>(
                name: "PicturePaths",
                table: "Blogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PicturePaths",
                table: "Blogs");

            migrationBuilder.AddColumn<string>(
                name: "Picture",
                table: "Blogs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
