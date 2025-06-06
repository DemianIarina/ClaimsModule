using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClaimsModule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingFieldsToPolicyAndClaim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "Policies");

            migrationBuilder.AddColumn<string>(
                name: "PolicyDocumentId",
                table: "Policies",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "AreasDamaged",
                table: "Claims",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DamageType",
                table: "Claims",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "IncidentLocation",
                table: "Claims",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "WasAnyoneInjured",
                table: "Claims",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Policies_PolicyDocumentId",
                table: "Policies",
                column: "PolicyDocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_PersistedDocument_PolicyDocumentId",
                table: "Policies",
                column: "PolicyDocumentId",
                principalTable: "PersistedDocument",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Policies_PersistedDocument_PolicyDocumentId",
                table: "Policies");

            migrationBuilder.DropIndex(
                name: "IX_Policies_PolicyDocumentId",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "PolicyDocumentId",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "AreasDamaged",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "DamageType",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "IncidentLocation",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "WasAnyoneInjured",
                table: "Claims");

            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "Policies",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
