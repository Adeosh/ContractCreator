using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractCreator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FirmEconomicActivities_Firms_EconomicActivityId",
                schema: "public",
                table: "FirmEconomicActivities");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                schema: "public",
                table: "GoodsAndServices",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "Type",
                schema: "public",
                table: "GoodsAndServices",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                schema: "public",
                table: "ContractSteps",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                schema: "public",
                table: "ContractStepItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                schema: "public",
                table: "ContractSpecifications",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                schema: "public",
                table: "ContractInvoiceItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                schema: "public",
                table: "ContractActItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_GoodsAndServices_CurrencyId",
                schema: "public",
                table: "GoodsAndServices",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsAndServices_Type",
                schema: "public",
                table: "GoodsAndServices",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ContractSteps_CurrencyId",
                schema: "public",
                table: "ContractSteps",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractStepItems_CurrencyId",
                schema: "public",
                table: "ContractStepItems",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractSpecifications_CurrencyId",
                schema: "public",
                table: "ContractSpecifications",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractInvoiceItems_CurrencyId",
                schema: "public",
                table: "ContractInvoiceItems",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractActItems_CurrencyId",
                schema: "public",
                table: "ContractActItems",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractActItems_ClassifierOkv_CurrencyId",
                schema: "public",
                table: "ContractActItems",
                column: "CurrencyId",
                principalSchema: "ref",
                principalTable: "ClassifierOkv",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractInvoiceItems_ClassifierOkv_CurrencyId",
                schema: "public",
                table: "ContractInvoiceItems",
                column: "CurrencyId",
                principalSchema: "ref",
                principalTable: "ClassifierOkv",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractSpecifications_ClassifierOkv_CurrencyId",
                schema: "public",
                table: "ContractSpecifications",
                column: "CurrencyId",
                principalSchema: "ref",
                principalTable: "ClassifierOkv",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractStepItems_ClassifierOkv_CurrencyId",
                schema: "public",
                table: "ContractStepItems",
                column: "CurrencyId",
                principalSchema: "ref",
                principalTable: "ClassifierOkv",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractSteps_ClassifierOkv_CurrencyId",
                schema: "public",
                table: "ContractSteps",
                column: "CurrencyId",
                principalSchema: "ref",
                principalTable: "ClassifierOkv",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsAndServices_ClassifierOkv_CurrencyId",
                schema: "public",
                table: "GoodsAndServices",
                column: "CurrencyId",
                principalSchema: "ref",
                principalTable: "ClassifierOkv",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractActItems_ClassifierOkv_CurrencyId",
                schema: "public",
                table: "ContractActItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractInvoiceItems_ClassifierOkv_CurrencyId",
                schema: "public",
                table: "ContractInvoiceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractSpecifications_ClassifierOkv_CurrencyId",
                schema: "public",
                table: "ContractSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractStepItems_ClassifierOkv_CurrencyId",
                schema: "public",
                table: "ContractStepItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractSteps_ClassifierOkv_CurrencyId",
                schema: "public",
                table: "ContractSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodsAndServices_ClassifierOkv_CurrencyId",
                schema: "public",
                table: "GoodsAndServices");

            migrationBuilder.DropIndex(
                name: "IX_GoodsAndServices_CurrencyId",
                schema: "public",
                table: "GoodsAndServices");

            migrationBuilder.DropIndex(
                name: "IX_GoodsAndServices_Type",
                schema: "public",
                table: "GoodsAndServices");

            migrationBuilder.DropIndex(
                name: "IX_ContractSteps_CurrencyId",
                schema: "public",
                table: "ContractSteps");

            migrationBuilder.DropIndex(
                name: "IX_ContractStepItems_CurrencyId",
                schema: "public",
                table: "ContractStepItems");

            migrationBuilder.DropIndex(
                name: "IX_ContractSpecifications_CurrencyId",
                schema: "public",
                table: "ContractSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_ContractInvoiceItems_CurrencyId",
                schema: "public",
                table: "ContractInvoiceItems");

            migrationBuilder.DropIndex(
                name: "IX_ContractActItems_CurrencyId",
                schema: "public",
                table: "ContractActItems");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                schema: "public",
                table: "GoodsAndServices");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "public",
                table: "GoodsAndServices");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                schema: "public",
                table: "ContractSteps");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                schema: "public",
                table: "ContractStepItems");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                schema: "public",
                table: "ContractSpecifications");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                schema: "public",
                table: "ContractInvoiceItems");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                schema: "public",
                table: "ContractActItems");

            migrationBuilder.AddForeignKey(
                name: "FK_FirmEconomicActivities_Firms_EconomicActivityId",
                schema: "public",
                table: "FirmEconomicActivities",
                column: "EconomicActivityId",
                principalSchema: "public",
                principalTable: "Firms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
