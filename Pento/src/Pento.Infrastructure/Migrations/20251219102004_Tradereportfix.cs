using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class Tradereportfix : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_trade_reports_user_reported_user_id",
            table: "trade_reports");

        migrationBuilder.DropIndex(
            name: "ix_trade_reports_reported_user_id",
            table: "trade_reports");

        migrationBuilder.DropColumn(
            name: "reported_user_id",
            table: "trade_reports");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "reported_user_id",
            table: "trade_reports",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty);

        migrationBuilder.CreateIndex(
            name: "ix_trade_reports_reported_user_id",
            table: "trade_reports",
            column: "reported_user_id");

        migrationBuilder.AddForeignKey(
            name: "fk_trade_reports_user_reported_user_id",
            table: "trade_reports",
            column: "reported_user_id",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }
}
