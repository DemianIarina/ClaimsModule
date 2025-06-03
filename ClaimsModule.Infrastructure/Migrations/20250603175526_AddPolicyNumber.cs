using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClaimsModule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPolicyNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PolicyNumber",
                table: "Policies",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PolicyNumber",
                table: "Policies");
        }
    }
}
