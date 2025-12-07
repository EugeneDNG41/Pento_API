using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class Notification : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "device_tokens",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                platform = table.Column<int>(type: "integer", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_device_tokens", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "notifications",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                body = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                type = table.Column<int>(type: "integer", nullable: false),
                data_json = table.Column<string>(type: "jsonb", nullable: true),
                status = table.Column<int>(type: "integer", nullable: false),
                sent_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                read_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_notifications", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_device_tokens_user_id",
            table: "device_tokens",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ux_device_tokens_token",
            table: "device_tokens",
            column: "token",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_notifications_status",
            table: "notifications",
            column: "status");

        migrationBuilder.CreateIndex(
            name: "ix_notifications_user_id",
            table: "notifications",
            column: "user_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "device_tokens");

        migrationBuilder.DropTable(
            name: "notifications");
    }
}
