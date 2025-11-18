using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RemoveEstimatedPrice : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "estimated_price",
            table: "grocery_list_items");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "estimated_price",
            table: "grocery_list_items",
            type: "numeric(12,2)",
            precision: 12,
            scale: 2,
            nullable: true);
    }
}
