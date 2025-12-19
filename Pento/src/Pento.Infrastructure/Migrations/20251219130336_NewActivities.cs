using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class NewActivities : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "activities",
            columns: ["code", "description", "name"],
            values: new object[,]
            {
                { "TRADE_OFFER_CREATE", "Creating a trade offer to exchange food items with other households.", "Create Trade Offer" },
                { "TRADE_REQUEST_CREATE", "Creating a trade request for offers from other households.", "Create Trade Request" }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "activities",
            keyColumn: "code",
            keyValue: "TRADE_OFFER_CREATE");

        migrationBuilder.DeleteData(
            table: "activities",
            keyColumn: "code",
            keyValue: "TRADE_REQUEST_CREATE");
    }
}
