using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractCreator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContractId1",
                schema: "public",
                table: "ContractSteps",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractSteps_ContractId1",
                schema: "public",
                table: "ContractSteps",
                column: "ContractId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractSteps_Contracts_ContractId1",
                schema: "public",
                table: "ContractSteps",
                column: "ContractId1",
                principalSchema: "public",
                principalTable: "Contracts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractSteps_Contracts_ContractId1",
                schema: "public",
                table: "ContractSteps");

            migrationBuilder.DropIndex(
                name: "IX_ContractSteps_ContractId1",
                schema: "public",
                table: "ContractSteps");

            migrationBuilder.DropColumn(
                name: "ContractId1",
                schema: "public",
                table: "ContractSteps");
        }
    }
}
