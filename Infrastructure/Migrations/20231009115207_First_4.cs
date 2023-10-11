using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class First_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QRCodes_Outlets_Id",
                table: "QRCodes");

            migrationBuilder.DropColumn(
                name: "Facebook",
                table: "Outlets");

            migrationBuilder.DropColumn(
                name: "Instagram",
                table: "Outlets");

            migrationBuilder.DropColumn(
                name: "Twitter",
                table: "Outlets");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "QRCodes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "TableId",
                table: "QRCodes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TableNumber",
                table: "QRCodes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Tables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TableName = table.Column<string>(type: "text", nullable: false),
                    TableNumber = table.Column<int>(type: "integer", nullable: false),
                    OutletId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tables_Outlets_OutletId",
                        column: x => x.OutletId,
                        principalTable: "Outlets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QRCodes_TableId",
                table: "QRCodes",
                column: "TableId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tables_OutletId",
                table: "Tables",
                column: "OutletId");

            migrationBuilder.AddForeignKey(
                name: "FK_QRCodes_Tables_TableId",
                table: "QRCodes",
                column: "TableId",
                principalTable: "Tables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QRCodes_Tables_TableId",
                table: "QRCodes");

            migrationBuilder.DropTable(
                name: "Tables");

            migrationBuilder.DropIndex(
                name: "IX_QRCodes_TableId",
                table: "QRCodes");

            migrationBuilder.DropColumn(
                name: "TableId",
                table: "QRCodes");

            migrationBuilder.DropColumn(
                name: "TableNumber",
                table: "QRCodes");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "QRCodes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "Facebook",
                table: "Outlets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Instagram",
                table: "Outlets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Twitter",
                table: "Outlets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_QRCodes_Outlets_Id",
                table: "QRCodes",
                column: "Id",
                principalTable: "Outlets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
