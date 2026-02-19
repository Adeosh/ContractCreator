using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ContractCreator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UploadDate",
                schema: "public",
                table: "Storages",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ChangeDate",
                schema: "public",
                table: "Storages",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ContractWaybills",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractId = table.Column<int>(type: "integer", nullable: false),
                    InvoiceId = table.Column<int>(type: "integer", nullable: false),
                    WaybillNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    WaybillDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    VATRate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    VATAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    AggregateAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyId = table.Column<int>(type: "integer", nullable: false),
                    ContractId1 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractWaybills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractWaybills_ClassifierOkv_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "ref",
                        principalTable: "ClassifierOkv",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractWaybills_ContractInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalSchema: "public",
                        principalTable: "ContractInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractWaybills_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "public",
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractWaybills_Contracts_ContractId1",
                        column: x => x.ContractId1,
                        principalSchema: "public",
                        principalTable: "Contracts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ContractWaybillItems",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WaybillId = table.Column<int>(type: "integer", nullable: false),
                    NomenclatureName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, computedColumnSql: "\"Quantity\" * \"UnitPrice\"", stored: true),
                    CurrencyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractWaybillItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractWaybillItems_ClassifierOkv_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "ref",
                        principalTable: "ClassifierOkv",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractWaybillItems_ContractWaybills_WaybillId",
                        column: x => x.WaybillId,
                        principalSchema: "public",
                        principalTable: "ContractWaybills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractWaybillItems_CurrencyId",
                schema: "public",
                table: "ContractWaybillItems",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractWaybillItems_WaybillId",
                schema: "public",
                table: "ContractWaybillItems",
                column: "WaybillId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractWaybills_ContractId",
                schema: "public",
                table: "ContractWaybills",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractWaybills_ContractId1",
                schema: "public",
                table: "ContractWaybills",
                column: "ContractId1");

            migrationBuilder.CreateIndex(
                name: "IX_ContractWaybills_CurrencyId",
                schema: "public",
                table: "ContractWaybills",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractWaybills_InvoiceId",
                schema: "public",
                table: "ContractWaybills",
                column: "InvoiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractWaybillItems",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ContractWaybills",
                schema: "public");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UploadDate",
                schema: "public",
                table: "Storages",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ChangeDate",
                schema: "public",
                table: "Storages",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
