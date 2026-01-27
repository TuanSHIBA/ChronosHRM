using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronos.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class altertableAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Employees_EmployeeId1",
                table: "Attendances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attendances",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_EmployeeId1",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "EmployeeId1",
                table: "Attendances");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attendances",
                table: "Attendances",
                column: "Date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Attendances",
                table: "Attendances");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Attendances",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "Attendances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId1",
                table: "Attendances",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attendances",
                table: "Attendances",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_EmployeeId1",
                table: "Attendances",
                column: "EmployeeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Employees_EmployeeId1",
                table: "Attendances",
                column: "EmployeeId1",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
