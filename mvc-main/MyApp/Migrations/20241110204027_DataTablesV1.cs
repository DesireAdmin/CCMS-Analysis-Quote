using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Migrations
{
    /// <inheritdoc />
    public partial class DataTablesV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuoteModels",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SMG_Vendor_PO = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SMG_CLIENT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StoreNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vendor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceRepName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsIncurredCost = table.Column<bool>(type: "bit", nullable: false),
                    SignatureUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IncurredBreakouts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CostType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Techs = table.Column<int>(type: "int", nullable: false),
                    InitialCallSubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuoteModelId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncurredBreakouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncurredBreakouts_QuoteModels_QuoteModelId",
                        column: x => x.QuoteModelId,
                        principalTable: "QuoteModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncurredTotals",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IncurredSubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IncurredTax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IncurredInitialCallTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuoteModelId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncurredTotals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncurredTotals_QuoteModels_QuoteModelId",
                        column: x => x.QuoteModelId,
                        principalTable: "QuoteModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProposedBreakouts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CostType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Techs = table.Column<int>(type: "int", nullable: false),
                    InitialCallSubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuoteModelId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposedBreakouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProposedBreakouts_QuoteModels_QuoteModelId",
                        column: x => x.QuoteModelId,
                        principalTable: "QuoteModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProposedTotals",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProposedSubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProposedTax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProposedInitialCallTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuoteModelId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposedTotals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProposedTotals_QuoteModels_QuoteModelId",
                        column: x => x.QuoteModelId,
                        principalTable: "QuoteModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncurredBreakouts_QuoteModelId",
                table: "IncurredBreakouts",
                column: "QuoteModelId");

            migrationBuilder.CreateIndex(
                name: "IX_IncurredTotals_QuoteModelId",
                table: "IncurredTotals",
                column: "QuoteModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProposedBreakouts_QuoteModelId",
                table: "ProposedBreakouts",
                column: "QuoteModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ProposedTotals_QuoteModelId",
                table: "ProposedTotals",
                column: "QuoteModelId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncurredBreakouts");

            migrationBuilder.DropTable(
                name: "IncurredTotals");

            migrationBuilder.DropTable(
                name: "ProposedBreakouts");

            migrationBuilder.DropTable(
                name: "ProposedTotals");

            migrationBuilder.DropTable(
                name: "QuoteModels");
        }
    }
}
