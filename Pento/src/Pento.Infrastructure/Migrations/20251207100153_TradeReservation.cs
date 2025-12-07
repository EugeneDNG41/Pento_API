using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class TradeReservation : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_food_item_reservations_giveaway_post_giveaway_post_id",
            table: "food_item_reservations");

        migrationBuilder.RenameColumn(
            name: "giveaway_post_id",
            table: "food_item_reservations",
            newName: "trade_item_id");

        migrationBuilder.RenameIndex(
            name: "ix_food_item_reservations_giveaway_post_id",
            table: "food_item_reservations",
            newName: "ix_food_item_reservations_trade_item_id");

        migrationBuilder.AddForeignKey(
            name: "fk_food_item_reservations_trade_item_trade_item_id",
            table: "food_item_reservations",
            column: "trade_item_id",
            principalTable: "trade_items",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_food_item_reservations_trade_item_trade_item_id",
            table: "food_item_reservations");

        migrationBuilder.RenameColumn(
            name: "trade_item_id",
            table: "food_item_reservations",
            newName: "giveaway_post_id");

        migrationBuilder.RenameIndex(
            name: "ix_food_item_reservations_trade_item_id",
            table: "food_item_reservations",
            newName: "ix_food_item_reservations_giveaway_post_id");

        migrationBuilder.AddForeignKey(
            name: "fk_food_item_reservations_giveaway_post_giveaway_post_id",
            table: "food_item_reservations",
            column: "giveaway_post_id",
            principalTable: "giveaway_posts",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
