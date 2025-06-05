using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClaimsModule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingRel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claims_Employees_EmployeeId",
                table: "Claims");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "Claims",
                newName: "AssignedEmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_Claims_EmployeeId",
                table: "Claims",
                newName: "IX_Claims_AssignedEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_Employees_AssignedEmployeeId",
                table: "Claims",
                column: "AssignedEmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claims_Employees_AssignedEmployeeId",
                table: "Claims");

            migrationBuilder.RenameColumn(
                name: "AssignedEmployeeId",
                table: "Claims",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_Claims_AssignedEmployeeId",
                table: "Claims",
                newName: "IX_Claims_EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_Employees_EmployeeId",
                table: "Claims",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");
        }
    }
}
