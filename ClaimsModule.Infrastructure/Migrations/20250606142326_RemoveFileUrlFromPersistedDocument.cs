using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClaimsModule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFileUrlFromPersistedDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "PersistedDocument",
                newName: "OriginalFileName");

            migrationBuilder.AddColumn<string>(
                name: "GeneratedFileName",
                table: "PersistedDocument",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeneratedFileName",
                table: "PersistedDocument");

            migrationBuilder.RenameColumn(
                name: "OriginalFileName",
                table: "PersistedDocument",
                newName: "FileName");
        }
    }
}
