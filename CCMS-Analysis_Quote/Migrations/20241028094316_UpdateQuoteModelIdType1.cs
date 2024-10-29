using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CCMS_Analysis_Quote.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuoteModelIdType1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuoteModel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    SMG_Vendor_PO = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SMG_CLIENT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LOCATION = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StoreNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CityState = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vendor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceRepName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsIncurredCost = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IncurredBreakouts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    CostType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Techs = table.Column<int>(type: "int", nullable: false),
                    InitialCallSubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuoteModelId = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncurredBreakouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncurredBreakouts_QuoteModel_QuoteModelId",
                        column: x => x.QuoteModelId,
                        principalTable: "QuoteModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProposedBreakouts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    CostType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Techs = table.Column<int>(type: "int", nullable: false),
                    InitialCallSubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuoteModelId = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposedBreakouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProposedBreakouts_QuoteModel_QuoteModelId",
                        column: x => x.QuoteModelId,
                        principalTable: "QuoteModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncurredBreakouts_QuoteModelId",
                table: "IncurredBreakouts",
                column: "QuoteModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ProposedBreakouts_QuoteModelId",
                table: "ProposedBreakouts",
                column: "QuoteModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncurredBreakouts");

            migrationBuilder.DropTable(
                name: "ProposedBreakouts");

            migrationBuilder.DropTable(
                name: "QuoteModel");
        }
    }
}
