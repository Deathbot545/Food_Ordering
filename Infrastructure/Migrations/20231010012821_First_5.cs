using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class First_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TableNumber",
                table: "Tables");

            migrationBuilder.RenameColumn(
                name: "TableName",
                table: "Tables",
                newName: "TableIdentifier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TableIdentifier",
                table: "Tables",
                newName: "TableName");

            migrationBuilder.AddColumn<int>(
                name: "TableNumber",
                table: "Tables",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
