using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class NewIndexes : Migration
{
    private static readonly string[] columns = new[] { "user_id", "subscription_id" };
    private static readonly string[] columnsArray = new[] { "user_subscription_id", "feature_code" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_user_subscriptions_user_id",
            table: "user_subscriptions");

        migrationBuilder.DropIndex(
            name: "ix_user_entitlements_user_subscription_id",
            table: "user_entitlements");

        migrationBuilder.AlterColumn<string>(
            name: "reservation_for",
            table: "food_item_reservations",
            type: "character varying(10)",
            maxLength: 10,
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer");

        migrationBuilder.CreateIndex(
            name: "ix_user_subscriptions_user_id_subscription_id",
            table: "user_subscriptions",
            columns: columns,
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_user_entitlements_user_subscription_id_feature_code",
            table: "user_entitlements",
            columns: columnsArray,
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_user_subscriptions_user_id_subscription_id",
            table: "user_subscriptions");

        migrationBuilder.DropIndex(
            name: "ix_user_entitlements_user_subscription_id_feature_code",
            table: "user_entitlements");

        migrationBuilder.AlterColumn<int>(
            name: "reservation_for",
            table: "food_item_reservations",
            type: "integer",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(10)",
            oldMaxLength: 10);

        migrationBuilder.CreateIndex(
            name: "ix_user_subscriptions_user_id",
            table: "user_subscriptions",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_entitlements_user_subscription_id",
            table: "user_entitlements",
            column: "user_subscription_id");
    }
}
