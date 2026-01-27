using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronos.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceStatusAndSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Attendances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApproverId",
                table: "Attendances",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerNote",
                table: "Attendances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Attendances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "Attendances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Attendances",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "ApproverId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "ManagerNote",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Attendances");
        }
    }
}
