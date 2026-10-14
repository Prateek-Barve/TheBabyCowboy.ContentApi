using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBabyCowboy.ContentApi.Migrations
{
    /// <inheritdoc />
    public partial class AddBlogExternalUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalUrl",
                table: "ContentBlogs",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalUrl",
                table: "ContentBlogs");
        }
    }
}
