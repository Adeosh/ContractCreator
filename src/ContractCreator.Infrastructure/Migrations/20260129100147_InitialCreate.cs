using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ContractCreator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.EnsureSchema(
                name: "ref");

            migrationBuilder.CreateTable(
                name: "ChangeHistories",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractId = table.Column<int>(type: "integer", nullable: false),
                    WorkerId = table.Column<int>(type: "integer", nullable: false),
                    StageTypeId = table.Column<int>(type: "integer", nullable: false),
                    ChangeDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClassifierBic",
                schema: "ref",
                columns: table => new
                {
                    BIC = table.Column<string>(type: "character(9)", fixedLength: true, maxLength: 9, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EnglishName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    RegistrationNumber = table.Column<string>(type: "text", nullable: true),
                    CountryCode = table.Column<string>(type: "text", nullable: true),
                    RegionCode = table.Column<byte>(type: "smallint", nullable: true),
                    PostalIndex = table.Column<string>(type: "text", nullable: true),
                    SettlementType = table.Column<string>(type: "text", nullable: true),
                    SettlementName = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ParentBIC = table.Column<string>(type: "text", nullable: true),
                    DateIn = table.Column<DateTime>(type: "date", nullable: false),
                    ParticipantType = table.Column<byte>(type: "smallint", nullable: true),
                    Services = table.Column<string>(type: "text", nullable: true),
                    ExchangeType = table.Column<string>(type: "text", nullable: true),
                    UID = table.Column<string>(type: "text", nullable: true),
                    ParticipantStatus = table.Column<string>(type: "text", nullable: true),
                    Account = table.Column<string>(type: "text", nullable: true),
                    AccountCBRBIC = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassifierBic", x => x.BIC);
                });

            migrationBuilder.CreateTable(
                name: "ClassifierGar",
                schema: "ref",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ObjectId = table.Column<long>(type: "bigint", nullable: false),
                    ObjectGuid = table.Column<Guid>(type: "uuid", nullable: true),
                    RegionCode = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    TypeName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Level = table.Column<short>(type: "smallint", nullable: false),
                    FullAddress = table.Column<string>(type: "text", nullable: false),
                    LiveInTown = table.Column<bool>(type: "boolean", nullable: false),
                    PostalIndex = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassifierGar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClassifierOkopf",
                schema: "ref",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassifierOkopf", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClassifierOkv",
                schema: "ref",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NumericCode = table.Column<int>(type: "integer", nullable: false),
                    LetterCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    CurrencyName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CountriesCurrencyUsed = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassifierOkv", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClassifierOkved",
                schema: "ref",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassifierOkved", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContractStages",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TypeIds = table.Column<int[]>(type: "integer[]", nullable: false),
                    Name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractStages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GoodsAndServices",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsAndServices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Storages",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StorageFileGuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UploadDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ChangeDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsEncrypted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Storages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Firms",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LegalFormType = table.Column<byte>(type: "smallint", nullable: false),
                    FullName = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    ShortName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    LegalAddress = table.Column<string>(type: "jsonb", nullable: false),
                    ActualAddress = table.Column<string>(type: "jsonb", nullable: false),
                    INN = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    KPP = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true),
                    OGRN = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    OKTMO = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    OKPO = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ERNS = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ExtraInformation = table.Column<string>(type: "text", nullable: true),
                    TaxationType = table.Column<byte>(type: "smallint", nullable: false),
                    IsVATPayment = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    UpdatedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    FacsimileSeal = table.Column<byte[]>(type: "bytea", nullable: true),
                    FacsimileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    OkopfId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Firms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Firms_ClassifierOkopf_OkopfId",
                        column: x => x.OkopfId,
                        principalSchema: "ref",
                        principalTable: "ClassifierOkopf",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FirmEconomicActivities",
                schema: "public",
                columns: table => new
                {
                    FirmId = table.Column<int>(type: "integer", nullable: false),
                    EconomicActivityId = table.Column<int>(type: "integer", nullable: false),
                    IsMain = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FirmEconomicActivities", x => new { x.FirmId, x.EconomicActivityId });
                    table.ForeignKey(
                        name: "FK_FirmEconomicActivities_ClassifierOkved_EconomicActivityId",
                        column: x => x.EconomicActivityId,
                        principalSchema: "ref",
                        principalTable: "ClassifierOkved",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FirmEconomicActivities_Firms_EconomicActivityId",
                        column: x => x.EconomicActivityId,
                        principalSchema: "public",
                        principalTable: "Firms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FirmEconomicActivities_Firms_FirmId",
                        column: x => x.FirmId,
                        principalSchema: "public",
                        principalTable: "Firms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FirmFiles",
                schema: "public",
                columns: table => new
                {
                    FirmId = table.Column<int>(type: "integer", nullable: false),
                    FileId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FirmFiles", x => new { x.FirmId, x.FileId });
                    table.ForeignKey(
                        name: "FK_FirmFiles_Firms_FirmId",
                        column: x => x.FirmId,
                        principalSchema: "public",
                        principalTable: "Firms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FirmFiles_Storages_FileId",
                        column: x => x.FileId,
                        principalSchema: "public",
                        principalTable: "Storages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Workers",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Position = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    INN = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    Phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    IsDirector = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsAccountant = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    FirmId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workers_Firms_FirmId",
                        column: x => x.FirmId,
                        principalSchema: "public",
                        principalTable: "Firms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BankAccounts",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BIC = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    BankName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    AccountNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CorrespondentAccount = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    BankAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    FirmId = table.Column<int>(type: "integer", nullable: true),
                    CounterpartyId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccounts", x => x.Id);
                    table.CheckConstraint("CK_BankAccount_Owner", "(\"FirmId\" IS NOT NULL AND \"CounterpartyId\" IS NULL) OR (\"FirmId\" IS NULL AND \"CounterpartyId\" IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_BankAccounts_Firms_FirmId",
                        column: x => x.FirmId,
                        principalSchema: "public",
                        principalTable: "Firms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Position = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    IsDirector = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsAccountant = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CounterpartyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Counterparties",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LegalForm = table.Column<byte>(type: "smallint", nullable: false),
                    FullName = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    ShortName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    LegalAddress = table.Column<string>(type: "jsonb", nullable: false),
                    ActualAddress = table.Column<string>(type: "jsonb", nullable: false),
                    INN = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    KPP = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true),
                    OGRN = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    OKTMO = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    OKPO = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ExtraInformation = table.Column<string>(type: "text", nullable: true),
                    DirectorId = table.Column<int>(type: "integer", nullable: true),
                    AccountantId = table.Column<int>(type: "integer", nullable: true),
                    FirmId = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    UpdatedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Counterparties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Counterparties_Contacts_AccountantId",
                        column: x => x.AccountantId,
                        principalSchema: "public",
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Counterparties_Contacts_DirectorId",
                        column: x => x.DirectorId,
                        principalSchema: "public",
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Counterparties_Firms_FirmId",
                        column: x => x.FirmId,
                        principalSchema: "public",
                        principalTable: "Firms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    EnterpriseRole = table.Column<byte>(type: "smallint", nullable: false),
                    ContractNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ContractPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ContractSubject = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    SubmissionDate = table.Column<DateOnly>(type: "date", nullable: true),
                    SubmissionCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SubmissionLink = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TenderDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IssueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExecutionDate = table.Column<DateOnly>(type: "date", nullable: true),
                    TerminationReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Initiator = table.Column<byte>(type: "smallint", nullable: true),
                    CounterpartyId = table.Column<int>(type: "integer", nullable: false),
                    CounterpartySignerId = table.Column<int>(type: "integer", nullable: true),
                    FirmId = table.Column<int>(type: "integer", nullable: false),
                    FirmSignerId = table.Column<int>(type: "integer", nullable: false),
                    CurrencyId = table.Column<int>(type: "integer", nullable: false),
                    StageTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contracts_ClassifierOkv_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "ref",
                        principalTable: "ClassifierOkv",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contracts_Contacts_CounterpartySignerId",
                        column: x => x.CounterpartySignerId,
                        principalSchema: "public",
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contracts_ContractStages_StageTypeId",
                        column: x => x.StageTypeId,
                        principalSchema: "public",
                        principalTable: "ContractStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contracts_Counterparties_CounterpartyId",
                        column: x => x.CounterpartyId,
                        principalSchema: "public",
                        principalTable: "Counterparties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contracts_Firms_FirmId",
                        column: x => x.FirmId,
                        principalSchema: "public",
                        principalTable: "Firms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contracts_Workers_FirmSignerId",
                        column: x => x.FirmSignerId,
                        principalSchema: "public",
                        principalTable: "Workers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CounterpartyFiles",
                schema: "public",
                columns: table => new
                {
                    CounterpartyId = table.Column<int>(type: "integer", nullable: false),
                    FileId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CounterpartyFiles", x => new { x.CounterpartyId, x.FileId });
                    table.ForeignKey(
                        name: "FK_CounterpartyFiles_Counterparties_CounterpartyId",
                        column: x => x.CounterpartyId,
                        principalSchema: "public",
                        principalTable: "Counterparties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CounterpartyFiles_Storages_FileId",
                        column: x => x.FileId,
                        principalSchema: "public",
                        principalTable: "Storages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractInvoices",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractId = table.Column<int>(type: "integer", nullable: false),
                    BankAccountId = table.Column<int>(type: "integer", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    InvoiceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PurchaserINN = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    PurchaserKPP = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    VATAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    VATRate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    AggregateAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyId = table.Column<int>(type: "integer", nullable: false),
                    CountNomenclatureNames = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractInvoices_BankAccounts_BankAccountId",
                        column: x => x.BankAccountId,
                        principalSchema: "public",
                        principalTable: "BankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractInvoices_ClassifierOkv_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "ref",
                        principalTable: "ClassifierOkv",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractInvoices_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "public",
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractSpecifications",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractId = table.Column<int>(type: "integer", nullable: false),
                    NomenclatureName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, computedColumnSql: "\"Quantity\" * \"UnitPrice\"", stored: true),
                    VATRate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractSpecifications_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "public",
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractSteps",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractId = table.Column<int>(type: "integer", nullable: false),
                    StepName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    StartStepDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndStepDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractSteps_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "public",
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractActs",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractId = table.Column<int>(type: "integer", nullable: false),
                    ActNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ActDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    VATRate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    VATAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    AggregateAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyId = table.Column<int>(type: "integer", nullable: false),
                    ContractorId = table.Column<int>(type: "integer", nullable: true),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    InvoiceId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractActs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractActs_ClassifierOkv_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "ref",
                        principalTable: "ClassifierOkv",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractActs_ContractInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalSchema: "public",
                        principalTable: "ContractInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractActs_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "public",
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractInvoiceItems",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InvoiceId = table.Column<int>(type: "integer", nullable: false),
                    NomenclatureName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, computedColumnSql: "\"Quantity\" * \"UnitPrice\"", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractInvoiceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractInvoiceItems_ContractInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalSchema: "public",
                        principalTable: "ContractInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractStepItems",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StepId = table.Column<int>(type: "integer", nullable: false),
                    NomenclatureName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, computedColumnSql: "\"Quantity\" * \"UnitPrice\"", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractStepItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractStepItems_ContractSteps_StepId",
                        column: x => x.StepId,
                        principalSchema: "public",
                        principalTable: "ContractSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractActItems",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActId = table.Column<int>(type: "integer", nullable: false),
                    NomenclatureName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, computedColumnSql: "\"Quantity\" * \"UnitPrice\"", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractActItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractActItems_ContractActs_ActId",
                        column: x => x.ActId,
                        principalSchema: "public",
                        principalTable: "ContractActs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "ref",
                table: "ClassifierOkv",
                columns: new[] { "Id", "CountriesCurrencyUsed", "CurrencyName", "LetterCode", "Note", "NumericCode" },
                values: new object[,]
                {
                    { 1, "Албания", "Лек", "ALL", null, 8 },
                    { 2, "Алжир", "Алжирский динар", "DZD", null, 12 },
                    { 3, "Аргентина", "Аргентинское песо", "ARS", null, 32 },
                    { 4, "Австралия; Кирибати; Кокосовые (Килинг) острова; Науру; Остров Норфолк; Остров Рождества; Остров Херд и острова Макдональд; Тувалу", "Австралийский доллар", "AUD", null, 36 },
                    { 5, "Багамы", "Багамский доллар", "BSD", null, 44 },
                    { 6, "Бахрейн", "Бахрейнский динар", "BHD", null, 48 },
                    { 7, "Бангладеш", "Така", "BDT", null, 50 },
                    { 8, "Армения", "Армянский драм", "AMD", null, 51 },
                    { 9, "Барбадос", "Барбадосский доллар", "BBD", null, 52 },
                    { 10, "Бермуды", "Бермудский доллар", "BMD", null, 60 },
                    { 11, "Бутан", "Нгултрум", "BTN", null, 64 },
                    { 12, "Боливия, многонациональное государство", "Боливиано", "BOB", null, 68 },
                    { 13, "Ботсвана", "Пула", "BWP", null, 72 },
                    { 14, "Белиз", "Белизский доллар", "BZD", null, 84 },
                    { 15, "Соломоновы Острова", "Доллар Соломоновых Островов", "SBD", null, 90 },
                    { 16, "Бруней-Даруссалам", "Брунейский доллар", "BND", null, 96 },
                    { 17, "Мьянма", "Кьят", "MMK", null, 104 },
                    { 18, "Бурунди", "Бурундийский франк", "BIF", null, 108 },
                    { 19, "Камбоджа", "Риель", "KHR", null, 116 },
                    { 20, "Канада", "Канадский доллар", "CAD", null, 124 },
                    { 21, "Кабо-Верде", "Эскудо Кабо-Верде", "CVE", null, 132 },
                    { 22, "Острова Кайман", "Доллар Островов Кайман", "KYD", null, 136 },
                    { 23, "Шри-Ланка", "Шри-ланкийская рупия", "LKR", null, 144 },
                    { 24, "Чили", "Чилийское песо", "CLP", null, 152 },
                    { 25, "Китай", "Юань", "CNY", null, 156 },
                    { 26, "Колумбия", "Колумбийское песо", "COP", null, 170 },
                    { 27, "Коморы", "Коморский франк", "KMF", null, 174 },
                    { 28, "Коста-Рика", "Коста-риканский колон", "CRC", null, 188 },
                    { 29, "Хорватия", "Куна", "HRK", null, 191 },
                    { 30, "Куба", "Кубинское песо", "CUP", null, 192 },
                    { 31, "Чехия", "Чешская крона", "CZK", null, 203 },
                    { 32, "Гренландия; Дания; Фарерские острова", "Датская крона", "DKK", null, 208 },
                    { 33, "Доминиканская Республика", "Доминиканское песо", "DOP", null, 214 },
                    { 34, "Эль-Сальвадор", "Сальвадорский колон", "SVC", null, 222 },
                    { 35, "Эфиопия", "Эфиопский быр", "ETB", null, 230 },
                    { 36, "Эритрея", "Накфа", "ERN", null, 232 },
                    { 37, "Фолклендские острова (Мальвинские)", "Фунт Фолклендских островов", "FKP", null, 238 },
                    { 38, "Фиджи", "Доллар Фиджи", "FJD", null, 242 },
                    { 39, "Джибути", "Франк Джибути", "DJF", null, 262 },
                    { 40, "Гамбия", "Даласи", "GMD", null, 270 },
                    { 41, "Гибралтар", "Гибралтарский фунт", "GIP", null, 292 },
                    { 42, "Гватемала", "Кетсаль", "GTQ", null, 320 },
                    { 43, "Гвинея", "Гвинейский франк", "GNF", null, 324 },
                    { 44, "Гайана", "Гайанский доллар", "GYD", null, 328 },
                    { 45, "Гаити", "Гурд", "HTG", null, 332 },
                    { 46, "Гондурас", "Лемпира", "HNL", null, 340 },
                    { 47, "Гонконг", "Гонконгский доллар", "HKD", null, 344 },
                    { 48, "Венгрия", "Форинт", "HUF", null, 348 },
                    { 49, "Исландия", "Исландская крона", "ISK", null, 352 },
                    { 50, "Бутан; Индия", "Индийская рупия", "INR", null, 356 },
                    { 51, "Индонезия", "Рупия", "IDR", null, 360 },
                    { 52, "Иран (Исламская Республика)", "Иранский риал", "IRR", null, 364 },
                    { 53, "Ирак", "Иракский динар", "IQD", null, 368 },
                    { 54, "Израиль", "Новый израильский шекель", "ILS", null, 376 },
                    { 55, "Ямайка", "Ямайский доллар", "JMD", null, 388 },
                    { 56, "Япония", "Иена", "JPY", null, 392 },
                    { 57, "Казахстан", "Тенге", "KZT", null, 398 },
                    { 58, "Иордания", "Иорданский динар", "JOD", null, 400 },
                    { 59, "Кения", "Кенийский шиллинг", "KES", null, 404 },
                    { 60, "Корея, народно-демократическая республика", "Северокорейская вона", "KPW", null, 408 },
                    { 61, "Корея, республика", "Вона", "KRW", null, 410 },
                    { 62, "Кувейт", "Кувейтский динар", "KWD", null, 414 },
                    { 63, "Киргизия", "Сом", "KGS", null, 417 },
                    { 64, "Лаосская Народно-Демократическая Республика", "Лаосский кип", "LAK", null, 418 },
                    { 65, "Ливан", "Ливанский фунт", "LBP", null, 422 },
                    { 66, "Лесото", "Лоти", "LSL", null, 426 },
                    { 67, "Либерия", "Либерийский доллар", "LRD", null, 430 },
                    { 68, "Ливия", "Ливийский динар", "LYD", null, 434 },
                    { 69, "Макао", "Патака", "MOP", null, 446 },
                    { 70, "Малави", "Малавийская квача", "MWK", null, 454 },
                    { 71, "Малайзия", "Малайзийский ринггит", "MYR", null, 458 },
                    { 72, "Мальдивы", "Руфия", "MVR", null, 462 },
                    { 73, "Маврикий", "Маврикийская рупия", "MUR", null, 480 },
                    { 74, "Мексика", "Мексиканское песо", "MXN", null, 484 },
                    { 75, "Монголия", "Тугрик", "MNT", null, 496 },
                    { 76, "Молдова, республика", "Молдавский лей", "MDL", null, 498 },
                    { 77, "Западная Сахара; Марокко", "Марокканский дирхам", "MAD", null, 504 },
                    { 78, "Оман", "Оманский риал", "OMR", null, 512 },
                    { 79, "Намибия", "Доллар Намибии", "NAD", null, 516 },
                    { 80, "Непал", "Непальская рупия", "NPR", null, 524 },
                    { 81, "Кюрасао; Сен-Мартен (нидерландская часть)", "Карибский гульден", "XCG", null, 532 },
                    { 82, "Аруба", "Арубанский флорин", "AWG", null, 533 },
                    { 83, "Вануату", "Вату", "VUV", null, 548 },
                    { 84, "Ниуэ; Новая Зеландия; Острова Кука; Питкерн; Токелау", "Новозеландский доллар", "NZD", null, 554 },
                    { 85, "Никарагуа", "Золотая кордоба", "NIO", null, 558 },
                    { 86, "Нигерия", "Найра", "NGN", null, 566 },
                    { 87, "Норвегия; Остров Буве; Шпицберген и Ян Майен", "Норвежская крона", "NOK", null, 578 },
                    { 88, "Пакистан", "Пакистанская рупия", "PKR", null, 586 },
                    { 89, "Панама", "Бальбоа", "PAB", null, 590 },
                    { 90, "Папуа Новая Гвинея", "Кина", "PGK", null, 598 },
                    { 91, "Парагвай", "Гуарани", "PYG", null, 600 },
                    { 92, "Перу", "Соль", "PEN", "Пояснение: изменение наименования действует с 15 декабря 2015 г.", 604 },
                    { 93, "Филиппины", "Филиппинское песо", "PHP", null, 608 },
                    { 94, "Катар", "Катарский риал", "QAR", null, 634 },
                    { 95, "Россия", "Российский рубль", "RUB", null, 643 },
                    { 96, "Руанда", "Франк Руанды", "RWF", null, 646 },
                    { 97, "Святая Елена, остров Вознесения, Тристан-да-Кунья", "Фунт Святой Елены", "SHP", null, 654 },
                    { 98, "Саудовская Аравия", "Саудовский риял", "SAR", null, 682 },
                    { 99, "Сейшелы", "Сейшельская рупия", "SCR", null, 690 },
                    { 100, "Сингапур", "Сингапурский доллар", "SGD", null, 702 },
                    { 101, "Вьетнам", "Донг", "VND", null, 704 },
                    { 102, "Сомали", "Сомалийский шиллинг", "SOS", null, 706 },
                    { 103, "Лесото; Намибия; Южная Африка", "Рэнд", "ZAR", null, 710 },
                    { 104, "Южный Судан", "Южносуданский фунт", "SSP", "данная валюта введена в действие для Южного Судана с 18 июля 2011 г.", 728 },
                    { 105, "Эсватини", "Лилангени", "SZL", null, 748 },
                    { 106, "Швеция", "Шведская крона", "SEK", null, 752 },
                    { 107, "Лихтенштейн; Швейцария", "Швейцарский франк", "CHF", null, 756 },
                    { 108, "Сирийская Арабская Республика", "Сирийский фунт", "SYP", null, 760 },
                    { 109, "Таиланд", "Бат", "THB", null, 764 },
                    { 110, "Тонга", "Паанга", "TOP", null, 776 },
                    { 111, "Тринидад и Тобаго", "Доллар Тринидада и Тобаго", "TTD", null, 780 },
                    { 112, "Объединенные Арабские Эмираты", "Дирхам ОАЭ", "AED", null, 784 },
                    { 113, "Тунис", "Тунисский динар", "TND", null, 788 },
                    { 114, "Уганда", "Угандийский шиллинг", "UGX", null, 800 },
                    { 115, "Северная Македония", "Денар", "MKD", null, 807 },
                    { 116, "Египет", "Египетский фунт", "EGP", null, 818 },
                    { 117, "Гернси; Джерси; Остров Мэн; Соединенное Королевство Великобритании и Северной Ирландии", "Фунт стерлингов", "GBP", null, 826 },
                    { 118, "Танзания, объединенная республика", "Танзанийский шиллинг", "TZS", null, 834 },
                    { 119, "Американские Самоа; Британская территория в Индийском океане; Бонэйр; Синт-Эстатиус и Саба; Виргинские острова (Британские); Виргинские острова (США); Гаити; Гуам; Малые Тихоокеанские Отдаленные острова Соединенных Штатов; Маршалловы Острова; Микронезия (федеративные штаты); Острова Теркс и Кайкос; Палау; Панама; Пуэрто-Рико; Северные Марианские острова; Соединенные Штаты Америки; Тимор-Лесте; Эквадор; Эль-Сальвадор", "Доллар США", "USD", null, 840 },
                    { 120, "Уругвай", "Уругвайское песо", "UYU", null, 858 },
                    { 121, "Узбекистан", "Узбекский сум", "UZS", null, 860 },
                    { 122, "Самоа", "Тала", "WST", null, 882 },
                    { 123, "Йемен", "Йеменский риал", "YER", null, 886 },
                    { 124, "Тайвань (Китай)", "Новый тайваньский доллар", "TWD", null, 901 },
                    { 125, "Зимбабве", "Зимбабвийский золотой", "ZWG", null, 924 },
                    { 126, "Сьерра-Леоне", "Леоне", "SLE", null, 925 },
                    { 127, "Венесуэла (Боливарианская Республика)", "Боливар Соберано", "VED", null, 926 },
                    { 128, "Венесуэла (Боливарианская Республика)", "Боливар Соберано", "VES", "Пояснение: деноминированная валюта", 928 },
                    { 129, "Мавритания", "Угия", "MRU", null, 929 },
                    { 130, "Сан-Томе и Принсипи", "Добра", "STN", null, 930 },
                    { 131, "Зимбабве", "Доллар Зимбабве", "ZWL", "Пояснение: данная валюта действует до 31 августа 2024 г. включительно", 932 },
                    { 132, "Беларусь", "Белорусский рубль", "BYN", null, 933 },
                    { 133, "Туркменистан", "Новый туркменский манат", "TMT", null, 934 },
                    { 134, "Гана", "Ганский седи", "GHS", null, 936 },
                    { 135, "Судан", "Суданский фунт", "SDG", null, 938 },
                    { 136, "Уругвай", "Уругвайское песо в индексированных единицах", "UYI", "данная валюта действует с 11 ноября 2006 г.", 940 },
                    { 137, "Сербия", "Сербский динар", "RSD", "данная валюта действует с 25 октября 2006 г.", 941 },
                    { 138, "Мозамбик", "Мозамбикский метикал", "MZN", null, 943 },
                    { 139, "Азербайджан", "Азербайджанский манат", "AZN", "данная валюта вводится в действие с 1 января 2006 г.", 944 },
                    { 140, "Румыния", "Румынский лей", "RON", null, 946 },
                    { 141, "Турция", "Турецкая лира", "TRY", "данная валюта введена в действие с 1 января 2009 г.", 949 },
                    { 142, "Габон; Камерун; Конго; Центрально-Африканская Республика; Чад; Экваториальная Гвинея", "Франк КФА ВЕАС", "XAF", "Франк КФА ВЕАС - денежная единица Банка государств Центральной Африки", 950 },
                    { 143, "Ангилья; Антигуа и Барбуда; Гренада; Доминика; Монтсеррат; Сент-Винсент и Гренадины; Сент-Китс и Невис; Сент-Люсия", "Восточно-карибский доллар", "XCD", null, 951 },
                    { 144, "Бенин; Буркина-Фасо; Гвинея-Бисау; Кот д'Ивуар; Мали; Нигер; Сенегал; Того", "Франк КФА ВСЕАО", "XOF", "Франк КФА ВСЕАО - денежная единица Центрального Банка государств Западной Африки", 952 },
                    { 145, "Новая Каледония; Французская Полинезия; Уоллис и Футуна", "Франк КФП", "XPF", null, 953 },
                    { 146, "Международный валютный фонд (МВФ)", "СДР (специальные права заимствования)", "XDR", null, 960 },
                    { 147, "Замбия", "Замбийская квача", "ZMW", null, 967 },
                    { 148, "Суринам", "Суринамский доллар", "SRD", null, 968 },
                    { 149, "Мадагаскар", "Малагасийский ариари", "MGA", null, 969 },
                    { 150, "Колумбия", "Единица реальной стоимости", "COU", null, 970 },
                    { 151, "Афганистан", "Афгани", "AFN", null, 971 },
                    { 152, "Таджикистан", "Сомони", "TJS", null, 972 },
                    { 153, "Ангола", "Кванза", "AOA", null, 973 },
                    { 154, "Болгария", "Болгарский лев", "BGN", null, 975 },
                    { 155, "Конго, демократическая республика", "Конголезский франк", "CDF", null, 976 },
                    { 156, "Босния и Герцеговина", "Конвертируемая марка", "ВАМ", null, 977 },
                    { 157, "Австрия; Аландские острова; Андорра; Бельгия; Гваделупа; Германия; Греция; Ирландия; Испания; Италия; Кипр; Латвия; Литва; Люксембург; Майотта; Мальта; Мартиника; Монако; Нидерланды; Папский Престол (Государство-город Ватикан); Португалия; Реюньон; Сан-Марино; Сен-Бартелеми; Сен-Мартен (французская часть); Сент-Пьер и Микелон; Словакия; Словения; Финляндия; Франция; Французская Гвиана; Французские Южные территории; Хорватия; Черногория; Эстония", "Евро", "EUR", null, 978 },
                    { 158, "Украина", "Гривна", "UAH", null, 980 },
                    { 159, "Грузия", "Лари", "GEL", null, 981 },
                    { 160, "Польша", "Злотый", "PLN", null, 985 },
                    { 161, "Бразилия", "Бразильский реал", "BRL", null, 986 }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "ContractStages",
                columns: new[] { "Id", "Description", "Name", "TypeIds" },
                values: new object[,]
                {
                    { 1, "Первоначальный статус при создании документа.", "Черновик", new[] { 1, 2 } },
                    { 2, "Согласование внутри предприятия и с контрагентом.", "Согласование", new[] { 1, 2 } },
                    { 3, "Заказчик разместил контракт для торгов.", "Подача заявок", new[] { 1 } },
                    { 4, "Проходят торги.", "Торги", new[] { 1 } },
                    { 5, "Предложение предприятия не выиграло на торгах.", "Торги проиграны", new[] { 1 } },
                    { 6, "Для договора: текст согласован сторонами, договор на подписи. Для госконтракта: торги выиграны, идет процесс заключения контракта.", "Заключение", new[] { 1, 2 } },
                    { 7, "Обе стороны подписали документ.", "Заключен", new[] { 1, 2 } },
                    { 8, "Находится на исполнении.", "На исполнении", new[] { 1, 2 } },
                    { 9, "Исполнен.", "Исполнен", new[] { 1, 2 } },
                    { 10, "Все финансовые и прочие расчеты по документу завершены.", "Оплачен", new[] { 1, 2 } },
                    { 11, "Договор на стадии расторжения.", "Расторжение", new[] { 2 } },
                    { 12, "Договор расторгнут.", "Расторгнут", new[] { 2 } }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_CounterpartyId",
                schema: "public",
                table: "BankAccounts",
                column: "CounterpartyId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_FirmId",
                schema: "public",
                table: "BankAccounts",
                column: "FirmId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeHistories_ContractId",
                schema: "public",
                table: "ChangeHistories",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassifierBic_Name",
                schema: "ref",
                table: "ClassifierBic",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "addresses_fulladdress_idx",
                schema: "ref",
                table: "ClassifierGar",
                column: "FullAddress");

            migrationBuilder.CreateIndex(
                name: "addresses_name_idx",
                schema: "ref",
                table: "ClassifierGar",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "addresses_objectid_idx",
                schema: "ref",
                table: "ClassifierGar",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassifierOkopf_Code",
                schema: "ref",
                table: "ClassifierOkopf",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassifierOkved_Code",
                schema: "ref",
                table: "ClassifierOkved",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_CounterpartyId",
                schema: "public",
                table: "Contacts",
                column: "CounterpartyId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractActItems_ActId",
                schema: "public",
                table: "ContractActItems",
                column: "ActId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractActs_ContractId",
                schema: "public",
                table: "ContractActs",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractActs_CurrencyId",
                schema: "public",
                table: "ContractActs",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractActs_InvoiceId",
                schema: "public",
                table: "ContractActs",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractInvoiceItems_InvoiceId",
                schema: "public",
                table: "ContractInvoiceItems",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractInvoices_BankAccountId",
                schema: "public",
                table: "ContractInvoices",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractInvoices_ContractId",
                schema: "public",
                table: "ContractInvoices",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractInvoices_CurrencyId",
                schema: "public",
                table: "ContractInvoices",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_CounterpartyId",
                schema: "public",
                table: "Contracts",
                column: "CounterpartyId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_CounterpartySignerId",
                schema: "public",
                table: "Contracts",
                column: "CounterpartySignerId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_CurrencyId",
                schema: "public",
                table: "Contracts",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_FirmId",
                schema: "public",
                table: "Contracts",
                column: "FirmId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_FirmSignerId",
                schema: "public",
                table: "Contracts",
                column: "FirmSignerId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_StageTypeId",
                schema: "public",
                table: "Contracts",
                column: "StageTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractSpecifications_ContractId",
                schema: "public",
                table: "ContractSpecifications",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractStepItems_StepId",
                schema: "public",
                table: "ContractStepItems",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractSteps_ContractId",
                schema: "public",
                table: "ContractSteps",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_Counterparties_AccountantId",
                schema: "public",
                table: "Counterparties",
                column: "AccountantId");

            migrationBuilder.CreateIndex(
                name: "IX_Counterparties_DirectorId",
                schema: "public",
                table: "Counterparties",
                column: "DirectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Counterparties_FirmId",
                schema: "public",
                table: "Counterparties",
                column: "FirmId");

            migrationBuilder.CreateIndex(
                name: "IX_CounterpartyFiles_FileId",
                schema: "public",
                table: "CounterpartyFiles",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_FirmEconomicActivities_EconomicActivityId",
                schema: "public",
                table: "FirmEconomicActivities",
                column: "EconomicActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_FirmFiles_FileId",
                schema: "public",
                table: "FirmFiles",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Firms_OkopfId",
                schema: "public",
                table: "Firms",
                column: "OkopfId");

            migrationBuilder.CreateIndex(
                name: "IX_Workers_FirmId",
                schema: "public",
                table: "Workers",
                column: "FirmId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Counterparties_CounterpartyId",
                schema: "public",
                table: "BankAccounts",
                column: "CounterpartyId",
                principalSchema: "public",
                principalTable: "Counterparties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Counterparties_CounterpartyId",
                schema: "public",
                table: "Contacts",
                column: "CounterpartyId",
                principalSchema: "public",
                principalTable: "Counterparties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Counterparties_CounterpartyId",
                schema: "public",
                table: "Contacts");

            migrationBuilder.DropTable(
                name: "ChangeHistories",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ClassifierBic",
                schema: "ref");

            migrationBuilder.DropTable(
                name: "ClassifierGar",
                schema: "ref");

            migrationBuilder.DropTable(
                name: "ContractActItems",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ContractInvoiceItems",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ContractSpecifications",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ContractStepItems",
                schema: "public");

            migrationBuilder.DropTable(
                name: "CounterpartyFiles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "FirmEconomicActivities",
                schema: "public");

            migrationBuilder.DropTable(
                name: "FirmFiles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "GoodsAndServices",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ContractActs",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ContractSteps",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ClassifierOkved",
                schema: "ref");

            migrationBuilder.DropTable(
                name: "Storages",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ContractInvoices",
                schema: "public");

            migrationBuilder.DropTable(
                name: "BankAccounts",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Contracts",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ClassifierOkv",
                schema: "ref");

            migrationBuilder.DropTable(
                name: "ContractStages",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Workers",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Counterparties",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Contacts",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Firms",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ClassifierOkopf",
                schema: "ref");
        }
    }
}
