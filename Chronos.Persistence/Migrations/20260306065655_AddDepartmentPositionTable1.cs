using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronos.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentPositionTable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentPosition_Departments_DepartmentId",
                table: "DepartmentPosition");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentPosition_Positions_PositionId",
                table: "DepartmentPosition");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DepartmentPosition",
                table: "DepartmentPosition");

            migrationBuilder.RenameTable(
                name: "DepartmentPosition",
                newName: "DepartmentPositions");

            migrationBuilder.RenameIndex(
                name: "IX_DepartmentPosition_PositionId",
                table: "DepartmentPositions",
                newName: "IX_DepartmentPositions_PositionId");

            migrationBuilder.RenameIndex(
                name: "IX_DepartmentPosition_DepartmentId",
                table: "DepartmentPositions",
                newName: "IX_DepartmentPositions_DepartmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DepartmentPositions",
                table: "DepartmentPositions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentPositions_Departments_DepartmentId",
                table: "DepartmentPositions",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentPositions_Positions_PositionId",
                table: "DepartmentPositions",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentPositions_Departments_DepartmentId",
                table: "DepartmentPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentPositions_Positions_PositionId",
                table: "DepartmentPositions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DepartmentPositions",
                table: "DepartmentPositions");

            migrationBuilder.RenameTable(
                name: "DepartmentPositions",
                newName: "DepartmentPosition");

            migrationBuilder.RenameIndex(
                name: "IX_DepartmentPositions_PositionId",
                table: "DepartmentPosition",
                newName: "IX_DepartmentPosition_PositionId");

            migrationBuilder.RenameIndex(
                name: "IX_DepartmentPositions_DepartmentId",
                table: "DepartmentPosition",
                newName: "IX_DepartmentPosition_DepartmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DepartmentPosition",
                table: "DepartmentPosition",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentPosition_Departments_DepartmentId",
                table: "DepartmentPosition",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentPosition_Positions_PositionId",
                table: "DepartmentPosition",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
