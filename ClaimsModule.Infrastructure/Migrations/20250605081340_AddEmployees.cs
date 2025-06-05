using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClaimsModule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResponsibleEmployeeId",
                table: "Policies",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeId",
                table: "Claims",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_ResponsibleEmployeeId",
                table: "Policies",
                column: "ResponsibleEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_EmployeeId",
                table: "Claims",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_Employees_EmployeeId",
                table: "Claims",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_Employees_ResponsibleEmployeeId",
                table: "Policies",
                column: "ResponsibleEmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claims_Employees_EmployeeId",
                table: "Claims");

            migrationBuilder.DropForeignKey(
                name: "FK_Policies_Employees_ResponsibleEmployeeId",
                table: "Policies");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Policies_ResponsibleEmployeeId",
                table: "Policies");

            migrationBuilder.DropIndex(
                name: "IX_Claims_EmployeeId",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "ResponsibleEmployeeId",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Claims");
        }
    }
}
