using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronos.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class altertableEmploymentContract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignedDate",
                table: "EmploymentContracts");

            migrationBuilder.RenameColumn(
                name: "AllowanceAmount",
                table: "EmploymentContracts",
                newName: "TravelAllowance");

            migrationBuilder.AlterColumn<decimal>(
                name: "InsuranceSalary",
                table: "EmploymentContracts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "EmploymentContracts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MealAllowance",
                table: "EmploymentContracts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "EmploymentContracts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherAllowance",
                table: "EmploymentContracts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "SignDate",
                table: "EmploymentContracts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "WorkingLocation",
                table: "EmploymentContracts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "EmploymentContracts");

            migrationBuilder.DropColumn(
                name: "MealAllowance",
                table: "EmploymentContracts");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "EmploymentContracts");

            migrationBuilder.DropColumn(
                name: "OtherAllowance",
                table: "EmploymentContracts");

            migrationBuilder.DropColumn(
                name: "SignDate",
                table: "EmploymentContracts");

            migrationBuilder.DropColumn(
                name: "WorkingLocation",
                table: "EmploymentContracts");

            migrationBuilder.RenameColumn(
                name: "TravelAllowance",
                table: "EmploymentContracts",
                newName: "AllowanceAmount");

            migrationBuilder.AlterColumn<decimal>(
                name: "InsuranceSalary",
                table: "EmploymentContracts",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<DateTime>(
                name: "SignedDate",
                table: "EmploymentContracts",
                type: "datetime2",
                nullable: true);
        }
    }
}
