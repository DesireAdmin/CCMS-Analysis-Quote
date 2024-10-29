using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CCMS_Analysis_Quote.Migrations
{
    /// <inheritdoc />
    public partial class AddSignatureUrlToQuoteModel1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SignatureUrl",
                table: "QuoteModels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignatureUrl",
                table: "QuoteModels");
        }
    }
}
