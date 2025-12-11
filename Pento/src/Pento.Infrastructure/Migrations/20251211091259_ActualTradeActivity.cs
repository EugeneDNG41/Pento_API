using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class ActualTradeActivity : Migration
{
    private static readonly string[] columns = new[] { "code", "description", "name" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "activities",
            columns: columns,
            values: new object[,]
            {
                { "FOOD_ITEM_TRADE_AWAY", "Giving a food item to another household through trade.", "TradeAway Out Food Item" },
                { "FOOD_ITEM_TRADE_IN", "Receiving a food item from another household through trade.", "TradeAway In Food Item" }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "activities",
            keyColumn: "code",
            keyValue: "FOOD_ITEM_TRADE_AWAY");

        migrationBuilder.DeleteData(
            table: "activities",
            keyColumn: "code",
            keyValue: "FOOD_ITEM_TRADE_IN");
    }
}
