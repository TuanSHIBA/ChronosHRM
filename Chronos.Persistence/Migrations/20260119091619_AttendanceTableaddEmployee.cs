using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronos.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AttendanceTableaddEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId1",
                table: "Attendances",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Employees_EmployeeId1",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_EmployeeId1",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "EmployeeId1",
                table: "Attendances");
        }
    }
}
