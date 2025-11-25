using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class ChangePaymentFK : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_payments_user_subscription_user_subscription_id",
            table: "payments");

        migrationBuilder.RenameColumn(
            name: "user_subscription_id",
            table: "payments",
            newName: "subscription_plan_id");

        migrationBuilder.RenameIndex(
            name: "ix_payments_user_subscription_id",
            table: "payments",
            newName: "ix_payments_subscription_plan_id");

        migrationBuilder.AddForeignKey(
            name: "fk_payments_subscription_plan_subscription_plan_id",
            table: "payments",
            column: "subscription_plan_id",
            principalTable: "subscription_plans",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_payments_subscription_plan_subscription_plan_id",
            table: "payments");

        migrationBuilder.RenameColumn(
            name: "subscription_plan_id",
            table: "payments",
            newName: "user_subscription_id");

        migrationBuilder.RenameIndex(
            name: "ix_payments_subscription_plan_id",
            table: "payments",
            newName: "ix_payments_user_subscription_id");

        migrationBuilder.AddForeignKey(
            name: "fk_payments_user_subscription_user_subscription_id",
            table: "payments",
            column: "user_subscription_id",
            principalTable: "user_subscriptions",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
