using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class TradeLocation : Migration
{
    private static readonly string[] columns = new[] { "code", "description", "name" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:PostgresExtension:postgis", ",,");

        migrationBuilder.AddColumn<Point>(
            name: "location",
            table: "trade_requests",
            type: "geometry",
            nullable: false);

        migrationBuilder.AddColumn<Point>(
            name: "location",
            table: "trade_offers",
            type: "geometry",
            nullable: false);

        migrationBuilder.InsertData(
            table: "activities",
            columns: columns,
            values: new object[] { "TRADE_COMPLETE", "Finalizing a trade between households to exchange food items.", "Complete Trade" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "activities",
            keyColumn: "code",
            keyValue: "TRADE_COMPLETE");

        migrationBuilder.DropColumn(
            name: "location",
            table: "trade_requests");

        migrationBuilder.DropColumn(
            name: "location",
            table: "trade_offers");

        migrationBuilder.AlterDatabase()
            .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");
    }
}
