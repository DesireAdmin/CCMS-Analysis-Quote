using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CCMS_Analysis_Quote.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuoteModelIdType4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncurredBreakouts_QuoteModel_QuoteModelId",
                table: "IncurredBreakouts");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposedBreakouts_QuoteModel_QuoteModelId",
                table: "ProposedBreakouts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuoteModel",
                table: "QuoteModel");

            migrationBuilder.RenameTable(
                name: "QuoteModel",
                newName: "QuoteModels");

            migrationBuilder.RenameColumn(
                name: "LOCATION",
                table: "QuoteModels",
                newName: "Location");

            migrationBuilder.AlterColumn<string>(
                name: "QuoteModelId",
                table: "ProposedBreakouts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)");

            migrationBuilder.AlterColumn<string>(
                name: "QuoteModelId",
                table: "IncurredBreakouts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "QuoteModels",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuoteModels",
                table: "QuoteModels",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IncurredBreakouts_QuoteModels_QuoteModelId",
                table: "IncurredBreakouts",
                column: "QuoteModelId",
                principalTable: "QuoteModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProposedBreakouts_QuoteModels_QuoteModelId",
                table: "ProposedBreakouts",
                column: "QuoteModelId",
                principalTable: "QuoteModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncurredBreakouts_QuoteModels_QuoteModelId",
                table: "IncurredBreakouts");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposedBreakouts_QuoteModels_QuoteModelId",
                table: "ProposedBreakouts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuoteModels",
                table: "QuoteModels");

            migrationBuilder.RenameTable(
                name: "QuoteModels",
                newName: "QuoteModel");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "QuoteModel",
                newName: "LOCATION");

            migrationBuilder.AlterColumn<string>(
                name: "QuoteModelId",
                table: "ProposedBreakouts",
                type: "nvarchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "QuoteModelId",
                table: "IncurredBreakouts",
                type: "nvarchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "QuoteModel",
                type: "nvarchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuoteModel",
                table: "QuoteModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IncurredBreakouts_QuoteModel_QuoteModelId",
                table: "IncurredBreakouts",
                column: "QuoteModelId",
                principalTable: "QuoteModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProposedBreakouts_QuoteModel_QuoteModelId",
                table: "ProposedBreakouts",
                column: "QuoteModelId",
                principalTable: "QuoteModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
