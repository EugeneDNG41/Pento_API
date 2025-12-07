using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class TradeUpdate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_trade_offers_trades_trade_id",
            table: "trade_offers");

        migrationBuilder.DropTable(
            name: "trade_wants");

        migrationBuilder.DropTable(
            name: "trades");

        migrationBuilder.DropPrimaryKey(
            name: "pk_trade_offers",
            table: "trade_offers");

        migrationBuilder.DropColumn(
            name: "trade_id",
            table: "trade_offers");

        migrationBuilder.DropColumn(
            name: "quantity",
            table: "trade_offers");

        migrationBuilder.RenameColumn(
            name: "trade_id",
            table: "trade_requests",
            newName: "trade_offer_id");

        migrationBuilder.RenameColumn(
            name: "unit_id",
            table: "trade_offers",
            newName: "user_id");

        migrationBuilder.RenameColumn(
            name: "food_item_id",
            table: "trade_offers",
            newName: "id");

        migrationBuilder.AlterColumn<string>(
            name: "status",
            table: "trade_requests",
            type: "character varying(20)",
            maxLength: 20,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "text");

        migrationBuilder.AddColumn<DateTime>(
            name: "created_on",
            table: "trade_offers",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<DateTime>(
            name: "end_date",
            table: "trade_offers",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<bool>(
            name: "is_deleted",
            table: "trade_offers",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<string>(
            name: "pickup_option",
            table: "trade_offers",
            type: "character varying(20)",
            maxLength: 20,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<DateTime>(
            name: "start_date",
            table: "trade_offers",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<string>(
            name: "status",
            table: "trade_offers",
            type: "character varying(20)",
            maxLength: 20,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<DateTime>(
            name: "updated_on",
            table: "trade_offers",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddPrimaryKey(
            name: "pk_trade_offers",
            table: "trade_offers",
            column: "id");

        migrationBuilder.CreateTable(
            name: "trade_sessions",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                trade_offer_id = table.Column<Guid>(type: "uuid", nullable: false),
                trade_request_id = table.Column<Guid>(type: "uuid", nullable: false),
                status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                started_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_trade_sessions", x => x.id);
                table.ForeignKey(
                    name: "fk_trade_sessions_trade_offers_trade_offer_id",
                    column: x => x.trade_offer_id,
                    principalTable: "trade_offers",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_trade_sessions_trade_requests_trade_request_id",
                    column: x => x.trade_request_id,
                    principalTable: "trade_requests",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "trade_items",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                food_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                quantity = table.Column<int>(type: "integer", nullable: false),
                unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                from = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                offer_id = table.Column<Guid>(type: "uuid", nullable: true),
                request_id = table.Column<Guid>(type: "uuid", nullable: true),
                session_id = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_trade_items", x => x.id);
                table.ForeignKey(
                    name: "fk_trade_items_trade_offer_offer_id",
                    column: x => x.offer_id,
                    principalTable: "trade_offers",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_trade_items_trade_request_request_id",
                    column: x => x.request_id,
                    principalTable: "trade_requests",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_trade_items_trade_session_session_id",
                    column: x => x.session_id,
                    principalTable: "trade_sessions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "trade_session_messages",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                trade_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                sender_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                message_text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                sent_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_trade_session_messages", x => x.id);
                table.ForeignKey(
                    name: "fk_trade_session_messages_trade_sessions_trade_session_id",
                    column: x => x.trade_session_id,
                    principalTable: "trade_sessions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_trade_requests_trade_offer_id",
            table: "trade_requests",
            column: "trade_offer_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_items_offer_id",
            table: "trade_items",
            column: "offer_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_items_request_id",
            table: "trade_items",
            column: "request_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_items_session_id",
            table: "trade_items",
            column: "session_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_session_messages_trade_session_id",
            table: "trade_session_messages",
            column: "trade_session_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_sessions_trade_offer_id",
            table: "trade_sessions",
            column: "trade_offer_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_sessions_trade_request_id",
            table: "trade_sessions",
            column: "trade_request_id");

        migrationBuilder.AddForeignKey(
            name: "fk_trade_requests_trade_offers_trade_offer_id",
            table: "trade_requests",
            column: "trade_offer_id",
            principalTable: "trade_offers",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_trade_requests_trade_offers_trade_offer_id",
            table: "trade_requests");

        migrationBuilder.DropTable(
            name: "trade_items");

        migrationBuilder.DropTable(
            name: "trade_session_messages");

        migrationBuilder.DropTable(
            name: "trade_sessions");

        migrationBuilder.DropIndex(
            name: "ix_trade_requests_trade_offer_id",
            table: "trade_requests");

        migrationBuilder.DropPrimaryKey(
            name: "pk_trade_offers",
            table: "trade_offers");

        migrationBuilder.DropColumn(
            name: "created_on",
            table: "trade_offers");

        migrationBuilder.DropColumn(
            name: "end_date",
            table: "trade_offers");

        migrationBuilder.DropColumn(
            name: "is_deleted",
            table: "trade_offers");

        migrationBuilder.DropColumn(
            name: "pickup_option",
            table: "trade_offers");

        migrationBuilder.DropColumn(
            name: "start_date",
            table: "trade_offers");

        migrationBuilder.DropColumn(
            name: "status",
            table: "trade_offers");

        migrationBuilder.DropColumn(
            name: "updated_on",
            table: "trade_offers");

        migrationBuilder.RenameColumn(
            name: "trade_offer_id",
            table: "trade_requests",
            newName: "trade_id");

        migrationBuilder.RenameColumn(
            name: "user_id",
            table: "trade_offers",
            newName: "unit_id");

        migrationBuilder.RenameColumn(
            name: "id",
            table: "trade_offers",
            newName: "food_item_id");

        migrationBuilder.AlterColumn<string>(
            name: "status",
            table: "trade_requests",
            type: "text",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(20)",
            oldMaxLength: 20);

        migrationBuilder.AddColumn<Guid>(
            name: "trade_id",
            table: "trade_offers",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty);

        migrationBuilder.AddColumn<int>(
            name: "quantity",
            table: "trade_offers",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddPrimaryKey(
            name: "pk_trade_offers",
            table: "trade_offers",
            columns: ["trade_id", "food_item_id"]);

        migrationBuilder.CreateTable(
            name: "trades",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                contact_info = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                pickup_option = table.Column<string>(type: "text", nullable: false),
                start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                status = table.Column<string>(type: "text", nullable: false),
                updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                user_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_trades", x => x.id);
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

        migrationBuilder.AddForeignKey(
            name: "fk_trade_offers_trades_trade_id",
            table: "trade_offers",
            column: "trade_id",
            principalTable: "trades",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
