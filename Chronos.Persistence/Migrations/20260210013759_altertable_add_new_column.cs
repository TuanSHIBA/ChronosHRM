using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronos.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class altertable_add_new_column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "EmploymentContracts");

            migrationBuilder.AlterColumn<string>(
                name: "WorkingLocation",
                table: "EmploymentContracts",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "EmploymentContracts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContractCode",
                table: "EmploymentContracts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "PositionId",
                table: "EmploymentContracts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentContracts_PositionId",
                table: "EmploymentContracts",
                column: "PositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmploymentContracts_Positions_PositionId",
                table: "EmploymentContracts",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmploymentContracts_Positions_PositionId",
                table: "EmploymentContracts");

            migrationBuilder.DropIndex(
                name: "IX_EmploymentContracts_PositionId",
                table: "EmploymentContracts");

            migrationBuilder.DropColumn(
                name: "PositionId",
                table: "EmploymentContracts");

            migrationBuilder.AlterColumn<string>(
                name: "WorkingLocation",
                table: "EmploymentContracts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "EmploymentContracts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContractCode",
                table: "EmploymentContracts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "EmploymentContracts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
