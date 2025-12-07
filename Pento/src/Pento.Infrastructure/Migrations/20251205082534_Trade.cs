using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class Trade : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "trade_requests",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                trade_id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                status = table.Column<string>(type: "text", nullable: false),
                created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_trade_requests", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "trades",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                status = table.Column<string>(type: "text", nullable: false),
                contact_info = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                pickup_option = table.Column<string>(type: "text", nullable: false),
                created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_trades", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "trade_offers",
            columns: table => new
            {
                trade_id = table.Column<Guid>(type: "uuid", nullable: false),
                food_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                quantity = table.Column<int>(type: "integer", nullable: false),
                unit_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_trade_offers", x => new { x.trade_id, x.food_item_id });
                table.ForeignKey(
                    name: "fk_trade_offers_trades_trade_id",
                    column: x => x.trade_id,
                    principalTable: "trades",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "trade_wants",
            columns: table => new
            {
                trade_id = table.Column<Guid>(type: "uuid", nullable: false),
                food_reference_id = table.Column<Guid>(type: "uuid", nullable: false),
                quantity = table.Column<int>(type: "integer", nullable: false),
                unit_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_trade_wants", x => new { x.trade_id, x.food_reference_id });
                table.ForeignKey(
                    name: "fk_trade_wants_trades_trade_id",
                    column: x => x.trade_id,
                    principalTable: "trades",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "trade_offers");

        migrationBuilder.DropTable(
            name: "trade_requests");

        migrationBuilder.DropTable(
            name: "trade_wants");

        migrationBuilder.DropTable(
            name: "trades");
    }
}
