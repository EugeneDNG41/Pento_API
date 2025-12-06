using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RefactorNoti : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_notifications_status",
            table: "notifications");

        migrationBuilder.DropColumn(
            name: "read_on_utc",
            table: "notifications");

        migrationBuilder.DropColumn(
            name: "status",
            table: "notifications");

        migrationBuilder.RenameColumn(
            name: "sent_on_utc",
            table: "notifications",
            newName: "read_on");

        migrationBuilder.AlterColumn<string>(
            name: "type",
            table: "notifications",
            type: "text",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer");

        migrationBuilder.AddColumn<DateTime>(
            name: "sent_on",
            table: "notifications",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "sent_on",
            table: "notifications");

        migrationBuilder.RenameColumn(
            name: "read_on",
            table: "notifications",
            newName: "sent_on_utc");

        migrationBuilder.AlterColumn<int>(
            name: "type",
            table: "notifications",
            type: "integer",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "text");

        migrationBuilder.AddColumn<DateTime>(
            name: "read_on_utc",
            table: "notifications",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "status",
            table: "notifications",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateIndex(
            name: "ix_notifications_status",
            table: "notifications",
            column: "status");
    }
}
