using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddPayment : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "payments",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                order_code = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                payment_link_id = table.Column<string>(type: "text", nullable: true),
                description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                amount = table.Column<long>(type: "bigint", nullable: false),
                amount_paid = table.Column<long>(type: "bigint", nullable: false),
                currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
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
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "payments");
    }
}
