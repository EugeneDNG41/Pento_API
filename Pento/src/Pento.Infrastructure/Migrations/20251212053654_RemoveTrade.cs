using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RemoveTrade : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_food_item_reservations_trade_item_trade_item_id",
            table: "food_item_reservations");

        migrationBuilder.DropTable(
            name: "trade_items");

        migrationBuilder.DropTable(
            name: "trade_session_item");

        migrationBuilder.DropTable(
            name: "trade_session_messages");

        migrationBuilder.DropTable(
            name: "trade_sessions");

        migrationBuilder.DropTable(
            name: "trade_requests");

        migrationBuilder.DropTable(
            name: "trade_offers");

        migrationBuilder.DropIndex(
            name: "ix_food_item_reservations_trade_item_id",
            table: "food_item_reservations");

        migrationBuilder.DropColumn(
            name: "trade_item_id",
            table: "food_item_reservations");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "trade_item_id",
            table: "food_item_reservations",
            type: "uuid",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "trade_offers",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                pickup_option = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                user_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_trade_offers", x => x.id);
                table.ForeignKey(
                    name: "fk_trade_offers_households_household_id",
                    column: x => x.household_id,
                    principalTable: "households",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_trade_offers_user_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "trade_requests",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                trade_offer_id = table.Column<Guid>(type: "uuid", nullable: false),
                updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                user_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_trade_requests", x => x.id);
                table.ForeignKey(
                    name: "fk_trade_requests_households_household_id",
                    column: x => x.household_id,
                    principalTable: "households",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_trade_requests_trade_offers_trade_offer_id",
                    column: x => x.trade_offer_id,
                    principalTable: "trade_offers",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_trade_requests_user_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "trade_items",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                food_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                from = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                quantity = table.Column<decimal>(type: "numeric", nullable: false),
                unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                offer_id = table.Column<Guid>(type: "uuid", nullable: true),
                request_id = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_trade_items", x => x.id);
                table.ForeignKey(
                    name: "fk_trade_items_food_items_food_item_id",
                    column: x => x.food_item_id,
                    principalTable: "food_items",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_trade_items_trade_offer_offer_id",
                    column: x => x.offer_id,
                    principalTable: "trade_offers",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_trade_items_trade_request_request_id",
                    column: x => x.request_id,
                    principalTable: "trade_requests",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_trade_items_units_unit_id",
                    column: x => x.unit_id,
                    principalTable: "units",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "trade_sessions",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                offer_household_id = table.Column<Guid>(type: "uuid", nullable: false),
                request_household_id = table.Column<Guid>(type: "uuid", nullable: false),
                started_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                trade_offer_id = table.Column<Guid>(type: "uuid", nullable: false),
                trade_request_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_trade_sessions", x => x.id);
                table.ForeignKey(
                    name: "fk_trade_sessions_households_offer_household_id",
                    column: x => x.offer_household_id,
                    principalTable: "households",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_trade_sessions_households_request_household_id",
                    column: x => x.request_household_id,
                    principalTable: "households",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_trade_sessions_trade_offers_trade_offer_id",
                    column: x => x.trade_offer_id,
                    principalTable: "trade_offers",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_trade_sessions_trade_requests_trade_request_id",
                    column: x => x.trade_request_id,
                    principalTable: "trade_requests",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "trade_session_item",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                food_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                from = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                quantity = table.Column<decimal>(type: "numeric", nullable: false),
                session_id = table.Column<Guid>(type: "uuid", nullable: false),
                unit_id = table.Column<Guid>(type: "uuid", nullable: false)
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

        migrationBuilder.CreateTable(
            name: "trade_session_messages",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                message_text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                sent_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                trade_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_trade_session_messages", x => x.id);
                table.ForeignKey(
                    name: "fk_trade_session_messages_trade_sessions_trade_session_id",
                    column: x => x.trade_session_id,
                    principalTable: "trade_sessions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_trade_session_messages_user_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "ix_food_item_reservations_trade_item_id",
            table: "food_item_reservations",
            column: "trade_item_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_items_food_item_id",
            table: "trade_items",
            column: "food_item_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_items_offer_id",
            table: "trade_items",
            column: "offer_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_items_request_id",
            table: "trade_items",
            column: "request_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_items_unit_id",
            table: "trade_items",
            column: "unit_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_offers_household_id",
            table: "trade_offers",
            column: "household_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_offers_user_id",
            table: "trade_offers",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_requests_household_id",
            table: "trade_requests",
            column: "household_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_requests_trade_offer_id",
            table: "trade_requests",
            column: "trade_offer_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_requests_user_id",
            table: "trade_requests",
            column: "user_id");

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

        migrationBuilder.CreateIndex(
            name: "ix_trade_session_messages_trade_session_id",
            table: "trade_session_messages",
            column: "trade_session_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_session_messages_user_id",
            table: "trade_session_messages",
            column: "user_id");


        migrationBuilder.CreateIndex(
            name: "ix_trade_sessions_offer_household_id",
            table: "trade_sessions",
            column: "offer_household_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_sessions_request_household_id",
            table: "trade_sessions",
            column: "request_household_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_sessions_trade_offer_id",
            table: "trade_sessions",
            column: "trade_offer_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_sessions_trade_request_id",
            table: "trade_sessions",
            column: "trade_request_id");

        migrationBuilder.AddForeignKey(
            name: "fk_food_item_reservations_trade_item_trade_item_id",
            table: "food_item_reservations",
            column: "trade_item_id",
            principalTable: "trade_items",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
