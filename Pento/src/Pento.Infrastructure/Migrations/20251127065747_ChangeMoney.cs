using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class ChangeMoney : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "price_currency",
            table: "subscription_plans",
            newName: "currency");

        migrationBuilder.RenameColumn(
            name: "price_amount",
            table: "subscription_plans",
            newName: "amount");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "currency",
            table: "subscription_plans",
            newName: "price_currency");

        migrationBuilder.RenameColumn(
            name: "amount",
            table: "subscription_plans",
            newName: "price_amount");
    }
}
