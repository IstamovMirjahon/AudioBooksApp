using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioBooks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class newComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "value",
                table: "comments",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "value",
                table: "comments");
        }
    }
}
