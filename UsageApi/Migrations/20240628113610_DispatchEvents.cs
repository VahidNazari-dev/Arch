using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsageApi.Migrations
{
    /// <inheritdoc />
    public partial class DispatchEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DispatchEventCount",
                table: "Usage01",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastDispatchEventDate",
                table: "Usage01",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastDispatchedEventName",
                table: "Usage01",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DispatchEventCount",
                table: "Usage01");

            migrationBuilder.DropColumn(
                name: "LastDispatchEventDate",
                table: "Usage01");

            migrationBuilder.DropColumn(
                name: "LastDispatchedEventName",
                table: "Usage01");
        }
    }
}
