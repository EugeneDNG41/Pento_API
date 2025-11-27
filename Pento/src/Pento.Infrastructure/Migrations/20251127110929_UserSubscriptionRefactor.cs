using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UserSubscriptionRefactor : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "cancelled_date_utc",
            table: "user_subscriptions");

        migrationBuilder.RenameColumn(
            name: "resumed_date",
            table: "user_subscriptions",
            newName: "cancelled_date");

        migrationBuilder.AddColumn<int>(
            name: "remaining_days_after_pause",
            table: "user_subscriptions",
            type: "integer",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "remaining_days_after_pause",
            table: "user_subscriptions");

        migrationBuilder.RenameColumn(
            name: "cancelled_date",
            table: "user_subscriptions",
            newName: "resumed_date");

        migrationBuilder.AddColumn<DateOnly>(
            name: "cancelled_date_utc",
            table: "user_subscriptions",
            type: "date",
            nullable: true);
    }
}
