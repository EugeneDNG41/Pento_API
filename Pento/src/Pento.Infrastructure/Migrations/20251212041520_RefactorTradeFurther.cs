using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RefactorTradeFurther : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_trade_sessions_user_offer_user_id",
            table: "trade_sessions");

        migrationBuilder.DropForeignKey(
            name: "fk_trade_sessions_user_request_user_id",
            table: "trade_sessions");

        migrationBuilder.RenameColumn(
            name: "request_user_id",
            table: "trade_sessions",
            newName: "request_household_id");

        migrationBuilder.RenameColumn(
            name: "offer_user_id",
            table: "trade_sessions",
            newName: "offer_household_id");

        migrationBuilder.RenameIndex(
            name: "ix_trade_sessions_request_user_id",
            table: "trade_sessions",
            newName: "ix_trade_sessions_request_household_id");

        migrationBuilder.RenameIndex(
            name: "ix_trade_sessions_offer_user_id",
            table: "trade_sessions",
            newName: "ix_trade_sessions_offer_household_id");

 



        migrationBuilder.AddForeignKey(
            name: "fk_trade_sessions_households_offer_household_id",
            table: "trade_sessions",
            column: "offer_household_id",
            principalTable: "households",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_trade_sessions_households_request_household_id",
            table: "trade_sessions",
            column: "request_household_id",
            principalTable: "households",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_trade_sessions_households_offer_household_id",
            table: "trade_sessions");

        migrationBuilder.DropForeignKey(
            name: "fk_trade_sessions_households_request_household_id",
            table: "trade_sessions");


        migrationBuilder.RenameColumn(
            name: "request_household_id",
            table: "trade_sessions",
            newName: "request_user_id");

        migrationBuilder.RenameColumn(
            name: "offer_household_id",
            table: "trade_sessions",
            newName: "offer_user_id");

        migrationBuilder.RenameIndex(
            name: "ix_trade_sessions_request_household_id",
            table: "trade_sessions",
            newName: "ix_trade_sessions_request_user_id");

        migrationBuilder.RenameIndex(
            name: "ix_trade_sessions_offer_household_id",
            table: "trade_sessions",
            newName: "ix_trade_sessions_offer_user_id");


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
}
