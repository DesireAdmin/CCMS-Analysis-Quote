using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Migrations
{
    /// <inheritdoc />
    public partial class TimeParts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimeNumber",
                table: "QuoteModels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeOption",
                table: "QuoteModels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TimeRangeFrom",
                table: "QuoteModels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimeRangeTo",
                table: "QuoteModels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeUnit",
                table: "QuoteModels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeNumber",
                table: "QuoteModels");

            migrationBuilder.DropColumn(
                name: "TimeOption",
                table: "QuoteModels");

            migrationBuilder.DropColumn(
                name: "TimeRangeFrom",
                table: "QuoteModels");

            migrationBuilder.DropColumn(
                name: "TimeRangeTo",
                table: "QuoteModels");

            migrationBuilder.DropColumn(
                name: "TimeUnit",
                table: "QuoteModels");
        }
    }
}
