using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractCreator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FirmId",
                schema: "public",
                table: "GoodsAndServices",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ContractFiles",
                schema: "public",
                columns: table => new
                {
                    ContractId = table.Column<int>(type: "integer", nullable: false),
                    FileId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractFiles", x => new { x.ContractId, x.FileId });
                    table.ForeignKey(
                        name: "FK_ContractFiles_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "public",
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractFiles_Storages_FileId",
                        column: x => x.FileId,
                        principalSchema: "public",
                        principalTable: "Storages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsAndServices_FirmId",
                schema: "public",
                table: "GoodsAndServices",
                column: "FirmId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractFiles_FileId",
                schema: "public",
                table: "ContractFiles",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsAndServices_Firms_FirmId",
                schema: "public",
                table: "GoodsAndServices",
                column: "FirmId",
                principalSchema: "public",
                principalTable: "Firms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoodsAndServices_Firms_FirmId",
                schema: "public",
                table: "GoodsAndServices");

            migrationBuilder.DropTable(
                name: "ContractFiles",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_GoodsAndServices_FirmId",
                schema: "public",
                table: "GoodsAndServices");

            migrationBuilder.DropColumn(
                name: "FirmId",
                schema: "public",
                table: "GoodsAndServices");
        }
    }
}
