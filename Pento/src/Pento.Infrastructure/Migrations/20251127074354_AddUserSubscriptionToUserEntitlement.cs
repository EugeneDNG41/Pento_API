using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddUserSubscriptionToUserEntitlement : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "user_subscription_id",
            table: "user_entitlements",
            type: "uuid",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "ix_user_entitlements_user_subscription_id",
            table: "user_entitlements",
            column: "user_subscription_id");

        migrationBuilder.AddForeignKey(
            name: "fk_user_entitlements_user_subscription_user_subscription_id",
            table: "user_entitlements",
            column: "user_subscription_id",
            principalTable: "user_subscriptions",
            principalColumn: "id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_user_entitlements_user_subscription_user_subscription_id",
            table: "user_entitlements");

        migrationBuilder.DropIndex(
            name: "ix_user_entitlements_user_subscription_id",
            table: "user_entitlements");

        migrationBuilder.DropColumn(
            name: "user_subscription_id",
            table: "user_entitlements");
    }
}
