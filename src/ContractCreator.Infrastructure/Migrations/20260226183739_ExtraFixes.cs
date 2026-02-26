using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractCreator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExtraFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractFiles_Storages_FileId",
                schema: "public",
                table: "ContractFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractSteps_Contracts_ContractId1",
                schema: "public",
                table: "ContractSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractWaybills_Contracts_ContractId1",
                schema: "public",
                table: "ContractWaybills");

            migrationBuilder.DropForeignKey(
                name: "FK_CounterpartyFiles_Storages_FileId",
                schema: "public",
                table: "CounterpartyFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_FirmFiles_Storages_FileId",
                schema: "public",
                table: "FirmFiles");

            migrationBuilder.DropIndex(
                name: "IX_ContractWaybills_ContractId1",
                schema: "public",
                table: "ContractWaybills");

            migrationBuilder.DropIndex(
                name: "IX_ContractSteps_ContractId1",
                schema: "public",
                table: "ContractSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Storages",
                schema: "public",
                table: "Storages");

            migrationBuilder.DropColumn(
                name: "ContractId1",
                schema: "public",
                table: "ContractWaybills");

            migrationBuilder.DropColumn(
                name: "ContractId1",
                schema: "public",
                table: "ContractSteps");

            migrationBuilder.RenameTable(
                name: "Storages",
                schema: "public",
                newName: "StorageFiles",
                newSchema: "public");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StorageFiles",
                schema: "public",
                table: "StorageFiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractFiles_StorageFiles_FileId",
                schema: "public",
                table: "ContractFiles",
                column: "FileId",
                principalSchema: "public",
                principalTable: "StorageFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CounterpartyFiles_StorageFiles_FileId",
                schema: "public",
                table: "CounterpartyFiles",
                column: "FileId",
                principalSchema: "public",
                principalTable: "StorageFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FirmFiles_StorageFiles_FileId",
                schema: "public",
                table: "FirmFiles",
                column: "FileId",
                principalSchema: "public",
                principalTable: "StorageFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractFiles_StorageFiles_FileId",
                schema: "public",
                table: "ContractFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CounterpartyFiles_StorageFiles_FileId",
                schema: "public",
                table: "CounterpartyFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_FirmFiles_StorageFiles_FileId",
                schema: "public",
                table: "FirmFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StorageFiles",
                schema: "public",
                table: "StorageFiles");

            migrationBuilder.RenameTable(
                name: "StorageFiles",
                schema: "public",
                newName: "Storages",
                newSchema: "public");

            migrationBuilder.AddColumn<int>(
                name: "ContractId1",
                schema: "public",
                table: "ContractWaybills",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContractId1",
                schema: "public",
                table: "ContractSteps",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Storages",
                schema: "public",
                table: "Storages",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ContractWaybills_ContractId1",
                schema: "public",
                table: "ContractWaybills",
                column: "ContractId1");

            migrationBuilder.CreateIndex(
                name: "IX_ContractSteps_ContractId1",
                schema: "public",
                table: "ContractSteps",
                column: "ContractId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractFiles_Storages_FileId",
                schema: "public",
                table: "ContractFiles",
                column: "FileId",
                principalSchema: "public",
                principalTable: "Storages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractSteps_Contracts_ContractId1",
                schema: "public",
                table: "ContractSteps",
                column: "ContractId1",
                principalSchema: "public",
                principalTable: "Contracts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractWaybills_Contracts_ContractId1",
                schema: "public",
                table: "ContractWaybills",
                column: "ContractId1",
                principalSchema: "public",
                principalTable: "Contracts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CounterpartyFiles_Storages_FileId",
                schema: "public",
                table: "CounterpartyFiles",
                column: "FileId",
                principalSchema: "public",
                principalTable: "Storages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FirmFiles_Storages_FileId",
                schema: "public",
                table: "FirmFiles",
                column: "FileId",
                principalSchema: "public",
                principalTable: "Storages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
