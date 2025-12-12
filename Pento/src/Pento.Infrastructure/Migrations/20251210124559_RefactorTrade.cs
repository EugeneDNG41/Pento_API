using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RefactorTrade : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "sender_user_id",
            table: "trade_session_messages",
            newName: "user_id");



        migrationBuilder.AddColumn<Guid>(
            name: "offer_user_id",
            table: "trade_sessions",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty);

        migrationBuilder.AddColumn<Guid>(
            name: "request_user_id",
            table: "trade_sessions",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty);

        migrationBuilder.AlterColumn<string>(
            name: "message_text",
            table: "trade_session_messages",
            type: "character varying(500)",
            maxLength: 500,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(2000)",
            oldMaxLength: 2000);

        migrationBuilder.AddColumn<Guid>(
            name: "household_id",
            table: "trade_requests",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty);

        migrationBuilder.AddColumn<Guid>(
            name: "household_id",
            table: "trade_offers",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty);

        migrationBuilder.AddColumn<string>(
            name: "item_from",
            table: "trade_items",
            type: "character varying(10)",
            maxLength: 10,
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "ix_trade_sessions_offer_user_id",
            table: "trade_sessions",
            column: "offer_user_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_sessions_request_user_id",
            table: "trade_sessions",
            column: "request_user_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_session_messages_user_id",
            table: "trade_session_messages",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_requests_household_id",
            table: "trade_requests",
            column: "household_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_requests_user_id",
            table: "trade_requests",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_offers_household_id",
            table: "trade_offers",
            column: "household_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_offers_user_id",
            table: "trade_offers",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_items_food_item_id",
            table: "trade_items",
            column: "food_item_id");

        migrationBuilder.AddForeignKey(
            name: "fk_trade_items_food_items_food_item_id",
            table: "trade_items",
            column: "food_item_id",
            principalTable: "food_items",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_trade_offers_households_household_id",
            table: "trade_offers",
            column: "household_id",
            principalTable: "households",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_trade_offers_user_user_id",
            table: "trade_offers",
            column: "user_id",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_trade_requests_households_household_id",
            table: "trade_requests",
            column: "household_id",
            principalTable: "households",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_trade_requests_user_user_id",
            table: "trade_requests",
            column: "user_id",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_trade_session_messages_user_user_id",
            table: "trade_session_messages",
            column: "user_id",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_trade_sessions_user_offer_user_id",
            table: "trade_sessions",
            column: "offer_user_id",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_trade_sessions_user_request_user_id",
            table: "trade_sessions",
            column: "request_user_id",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_trade_items_food_items_food_item_id",
            table: "trade_items");

        migrationBuilder.DropForeignKey(
            name: "fk_trade_offers_households_household_id",
            table: "trade_offers");

        migrationBuilder.DropForeignKey(
            name: "fk_trade_offers_user_user_id",
            table: "trade_offers");

        migrationBuilder.DropForeignKey(
            name: "fk_trade_requests_households_household_id",
            table: "trade_requests");

        migrationBuilder.DropForeignKey(
            name: "fk_trade_requests_user_user_id",
            table: "trade_requests");

        migrationBuilder.DropForeignKey(
            name: "fk_trade_session_messages_user_user_id",
            table: "trade_session_messages");

        migrationBuilder.DropForeignKey(
            name: "fk_trade_sessions_user_offer_user_id",
            table: "trade_sessions");

        migrationBuilder.DropForeignKey(
            name: "fk_trade_sessions_user_request_user_id",
            table: "trade_sessions");

        migrationBuilder.DropIndex(
            name: "ix_trade_sessions_offer_user_id",
            table: "trade_sessions");

        migrationBuilder.DropIndex(
            name: "ix_trade_sessions_request_user_id",
            table: "trade_sessions");

        migrationBuilder.DropIndex(
            name: "ix_trade_session_messages_user_id",
            table: "trade_session_messages");

        migrationBuilder.DropIndex(
            name: "ix_trade_requests_household_id",
            table: "trade_requests");

        migrationBuilder.DropIndex(
            name: "ix_trade_requests_user_id",
            table: "trade_requests");

        migrationBuilder.DropIndex(
            name: "ix_trade_offers_household_id",
            table: "trade_offers");

        migrationBuilder.DropIndex(
            name: "ix_trade_offers_user_id",
            table: "trade_offers");

        migrationBuilder.DropIndex(
            name: "ix_trade_items_food_item_id",
            table: "trade_items");


        migrationBuilder.DropColumn(
            name: "offer_user_id",
            table: "trade_sessions");

        migrationBuilder.DropColumn(
            name: "request_user_id",
            table: "trade_sessions");

        migrationBuilder.DropColumn(
            name: "household_id",
            table: "trade_requests");

        migrationBuilder.DropColumn(
            name: "household_id",
            table: "trade_offers");

        migrationBuilder.DropColumn(
            name: "item_from",
            table: "trade_items");

        migrationBuilder.RenameColumn(
            name: "user_id",
            table: "trade_session_messages",
            newName: "sender_user_id");

        migrationBuilder.AlterColumn<string>(
            name: "message_text",
            table: "trade_session_messages",
            type: "character varying(2000)",
            maxLength: 2000,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(500)",
            oldMaxLength: 500);
    }
}
