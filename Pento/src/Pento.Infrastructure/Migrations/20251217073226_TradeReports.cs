using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class TradeReports : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "trade_reports",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                trade_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                reporter_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                reported_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                reason = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_trade_reports", x => x.id);
                table.ForeignKey(
                    name: "fk_trade_reports_user_reported_user_id",
                    column: x => x.reported_user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_trade_reports_user_reporter_user_id",
                    column: x => x.reporter_user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "trade_report_medias",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                trade_report_id = table.Column<Guid>(type: "uuid", nullable: false),
                media_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                media_uri = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_trade_report_medias", x => x.id);
                table.ForeignKey(
                    name: "fk_trade_report_medias_trade_reports_trade_report_id",
                    column: x => x.trade_report_id,
                    principalTable: "trade_reports",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_trade_report_medias_trade_report_id",
            table: "trade_report_medias",
            column: "trade_report_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_reports_reported_user_id",
            table: "trade_reports",
            column: "reported_user_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_reports_reporter_user_id",
            table: "trade_reports",
            column: "reporter_user_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "trade_report_medias");

        migrationBuilder.DropTable(
            name: "trade_reports");

    }
}
