using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class SubscriptionAndPayment : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "subscriptions",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                is_archived = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_subscriptions", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "user_entitlements",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                feature = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                usage_count = table.Column<int>(type: "integer", nullable: false),
                limit_quota = table.Column<int>(type: "integer", nullable: true),
                limit_reset_per = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                is_archived = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_entitlements", x => x.id);
                table.ForeignKey(
                    name: "fk_user_entitlements_users_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "subscription_features",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                subscription_id = table.Column<Guid>(type: "uuid", nullable: false),
                feature = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                limit_quota = table.Column<int>(type: "integer", nullable: true),
                limit_reset_per = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                is_archived = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_subscription_features", x => x.id);
                table.ForeignKey(
                    name: "fk_subscription_features_subscriptions_subscription_id",
                    column: x => x.subscription_id,
                    principalTable: "subscriptions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "subscription_plans",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                subscription_id = table.Column<Guid>(type: "uuid", nullable: false),
                price_amount = table.Column<long>(type: "bigint", nullable: false),
                price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                duration_value = table.Column<int>(type: "integer", nullable: false),
                duration_unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                is_archived = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_subscription_plans", x => x.id);
                table.ForeignKey(
                    name: "fk_subscription_plans_subscriptions_subscription_id",
                    column: x => x.subscription_id,
                    principalTable: "subscriptions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "user_subscriptions",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                subscription_id = table.Column<Guid>(type: "uuid", nullable: false),
                status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                start_date = table.Column<DateOnly>(type: "date", nullable: false),
                end_date = table.Column<DateOnly>(type: "date", nullable: true),
                paused_date = table.Column<DateOnly>(type: "date", nullable: true),
                resumed_date = table.Column<DateOnly>(type: "date", nullable: true),
                cancelled_date_utc = table.Column<DateOnly>(type: "date", nullable: true),
                cancellation_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                is_archived = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_subscriptions", x => x.id);
                table.ForeignKey(
                    name: "fk_user_subscriptions_subscriptions_subscription_id",
                    column: x => x.subscription_id,
                    principalTable: "subscriptions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_user_subscriptions_users_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "payments",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                user_subscription_id = table.Column<Guid>(type: "uuid", nullable: false),
                order_code = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                payment_link_id = table.Column<string>(type: "text", nullable: true),
                description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                amount_due = table.Column<long>(type: "bigint", nullable: false),
                amount_paid = table.Column<long>(type: "bigint", nullable: false),
                currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                status = table.Column<int>(type: "integer", nullable: false),
                checkout_url = table.Column<string>(type: "text", nullable: true),
                qr_code = table.Column<string>(type: "text", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                paid_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                cancelled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                cancellation_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                is_archived = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_payments", x => x.id);
                table.ForeignKey(
                    name: "fk_payments_user_subscription_user_subscription_id",
                    column: x => x.user_subscription_id,
                    principalTable: "user_subscriptions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_payments_user_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_payments_user_id",
            table: "payments",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_payments_user_subscription_id",
            table: "payments",
            column: "user_subscription_id");

        migrationBuilder.CreateIndex(
            name: "ix_subscription_features_subscription_id",
            table: "subscription_features",
            column: "subscription_id");

        migrationBuilder.CreateIndex(
            name: "ix_subscription_plans_subscription_id",
            table: "subscription_plans",
            column: "subscription_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_entitlements_user_id",
            table: "user_entitlements",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_subscriptions_subscription_id",
            table: "user_subscriptions",
            column: "subscription_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_subscriptions_user_id",
            table: "user_subscriptions",
            column: "user_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "payments");

        migrationBuilder.DropTable(
            name: "subscription_features");

        migrationBuilder.DropTable(
            name: "subscription_plans");

        migrationBuilder.DropTable(
            name: "user_entitlements");

        migrationBuilder.DropTable(
            name: "user_subscriptions");

        migrationBuilder.DropTable(
            name: "subscriptions");
    }
}
