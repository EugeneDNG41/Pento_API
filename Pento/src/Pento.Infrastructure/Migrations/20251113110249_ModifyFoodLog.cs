using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class ModifyFoodLog : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "base_unit_type",
            table: "food_item_logs");

        migrationBuilder.RenameColumn(
            name: "base_quantity",
            table: "food_item_logs",
            newName: "quantity");

        migrationBuilder.AlterColumn<string>(
            name: "action",
            table: "food_item_logs",
            type: "character varying(50)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer");

        migrationBuilder.AddColumn<Guid>(
            name: "unit_id",
            table: "food_item_logs",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty);

        migrationBuilder.CreateIndex(
            name: "ix_food_item_logs_unit_id",
            table: "food_item_logs",
            column: "unit_id");

        migrationBuilder.AddForeignKey(
            name: "fk_food_item_logs_units_unit_id",
            table: "food_item_logs",
            column: "unit_id",
            principalTable: "units",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_food_item_logs_units_unit_id",
            table: "food_item_logs");

        migrationBuilder.DropIndex(
            name: "ix_food_item_logs_unit_id",
            table: "food_item_logs");

        migrationBuilder.DropColumn(
            name: "unit_id",
            table: "food_item_logs");

        migrationBuilder.RenameColumn(
            name: "quantity",
            table: "food_item_logs",
            newName: "base_quantity");

        migrationBuilder.AlterColumn<int>(
            name: "action",
            table: "food_item_logs",
            type: "integer",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(50)",
            oldMaxLength: 50);

        migrationBuilder.AddColumn<string>(
            name: "base_unit_type",
            table: "food_item_logs",
            type: "character varying(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "");
    }
}
