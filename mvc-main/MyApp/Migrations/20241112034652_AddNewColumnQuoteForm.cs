using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColumnQuoteForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IncurredIssueDescription",
                table: "QuoteModels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProposedIssueDescription",
                table: "QuoteModels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ServiceDate",
                table: "QuoteModels",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncurredIssueDescription",
                table: "QuoteModels");

            migrationBuilder.DropColumn(
                name: "ProposedIssueDescription",
                table: "QuoteModels");

            migrationBuilder.DropColumn(
                name: "ServiceDate",
                table: "QuoteModels");
        }
    }
}
