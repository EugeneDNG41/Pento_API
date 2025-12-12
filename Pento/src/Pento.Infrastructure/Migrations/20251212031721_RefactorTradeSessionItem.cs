using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RefactorTradeSessionItem : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_trade_items_trade_session_session_id",
            table: "trade_items");

        migrationBuilder.DropIndex(
            name: "ix_trade_items_session_id",
            table: "trade_items");

        migrationBuilder.DropColumn(
            name: "item_from",
            table: "trade_items");

        migrationBuilder.DropColumn(
            name: "session_id",
            table: "trade_items");

        migrationBuilder.CreateTable(
            name: "trade_session_item",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                food_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                quantity = table.Column<decimal>(type: "numeric", nullable: false),
                unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                from = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                session_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_trade_session_item", x => x.id);
                table.ForeignKey(
                    name: "fk_trade_session_item_food_items_food_item_id",
                    column: x => x.food_item_id,
                    principalTable: "food_items",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_trade_session_item_trade_session_session_id",
                    column: x => x.session_id,
                    principalTable: "trade_sessions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_trade_session_item_units_unit_id",
                    column: x => x.unit_id,
                    principalTable: "units",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "ix_trade_items_unit_id",
            table: "trade_items",
            column: "unit_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_session_item_food_item_id",
            table: "trade_session_item",
            column: "food_item_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_session_item_session_id",
            table: "trade_session_item",
            column: "session_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_session_item_unit_id",
            table: "trade_session_item",
            column: "unit_id");

        migrationBuilder.AddForeignKey(
            name: "fk_trade_items_units_unit_id",
            table: "trade_items",
            column: "unit_id",
            principalTable: "units",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_trade_items_units_unit_id",
            table: "trade_items");

        migrationBuilder.DropTable(
            name: "trade_session_item");

        migrationBuilder.DropIndex(
            name: "ix_trade_items_unit_id",
            table: "trade_items");

        migrationBuilder.AddColumn<string>(
            name: "item_from",
            table: "trade_items",
            type: "character varying(10)",
            maxLength: 10,
            nullable: true);

        migrationBuilder.AddColumn<Guid>(
            name: "session_id",
            table: "trade_items",
            type: "uuid",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "ix_trade_items_session_id",
            table: "trade_items",
            column: "session_id");

        migrationBuilder.AddForeignKey(
            name: "fk_trade_items_trade_session_session_id",
            table: "trade_items",
            column: "session_id",
            principalTable: "trade_sessions",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
