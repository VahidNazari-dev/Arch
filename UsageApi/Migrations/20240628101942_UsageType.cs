using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsageApi.Migrations
{
    /// <inheritdoc />
    public partial class UsageType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Usage01",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Usage01");
        }
    }
}
