using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ContractCreator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTemplatesAndDeleteContractRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnterpriseRole",
                schema: "public",
                table: "Contracts");

            migrationBuilder.CreateTable(
                name: "DocumentTemplates",
                schema: "ref",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TemplateName = table.Column<string>(type: "text", nullable: false),
                    TemplateType = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTemplates", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentTemplates",
                schema: "ref");

            migrationBuilder.AddColumn<byte>(
                name: "EnterpriseRole",
                schema: "public",
                table: "Contracts",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
