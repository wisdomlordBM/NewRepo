using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jobportalwebsite.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyIdToBlogModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Blogs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_CompanyId",
                table: "Blogs",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Companies_CompanyId",
                table: "Blogs",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Companies_CompanyId",
                table: "Blogs");

            migrationBuilder.DropIndex(
                name: "IX_Blogs_CompanyId",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Blogs");
        }
    }
}
