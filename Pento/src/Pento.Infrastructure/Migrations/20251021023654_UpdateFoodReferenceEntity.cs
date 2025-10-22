using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UpdateFoodReferenceEntity : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "open_food_facts_id",
            table: "food_references");

        migrationBuilder.AlterColumn<string>(
            name: "usda_id",
            table: "food_references",
            type: "character varying(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "character varying(100)",
            oldMaxLength: 100,
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "typical_shelf_life_days",
            table: "food_references",
            type: "integer",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "integer");

        migrationBuilder.AlterColumn<string>(
            name: "name",
            table: "food_references",
            type: "character varying(255)",
            maxLength: 255,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(200)",
            oldMaxLength: 200);

        migrationBuilder.Sql(@"
    ALTER TABLE food_references
    ALTER COLUMN food_group TYPE character varying(50)
    USING food_group::text;
");

        migrationBuilder.AlterColumn<string>(
            name: "brand",
            table: "food_references",
            type: "character varying(200)",
            maxLength: 200,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "character varying(100)",
            oldMaxLength: 100,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "barcode",
            table: "food_references",
            type: "character varying(100)",
            maxLength: 100,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "character varying(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "data_type",
            table: "food_references",
            type: "character varying(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<int>(
            name: "food_category_id",
            table: "food_references",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "image_url",
            table: "food_references",
            type: "character varying(500)",
            maxLength: 500,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "published_on_utc",
            table: "food_references",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "data_type",
            table: "food_references");

        migrationBuilder.DropColumn(
            name: "food_category_id",
            table: "food_references");

        migrationBuilder.DropColumn(
            name: "image_url",
            table: "food_references");

        migrationBuilder.DropColumn(
            name: "published_on_utc",
            table: "food_references");

        migrationBuilder.AlterColumn<string>(
            name: "usda_id",
            table: "food_references",
            type: "character varying(100)",
            maxLength: 100,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "character varying(100)",
            oldMaxLength: 100);

        migrationBuilder.AlterColumn<int>(
            name: "typical_shelf_life_days",
            table: "food_references",
            type: "integer",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer",
            oldDefaultValue: 0);

        migrationBuilder.AlterColumn<string>(
            name: "name",
            table: "food_references",
            type: "character varying(200)",
            maxLength: 200,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(255)",
            oldMaxLength: 255);

        migrationBuilder.AlterColumn<string>(
            name: "food_group",
            table: "food_references",
            type: "text",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer");

        migrationBuilder.AlterColumn<string>(
            name: "brand",
            table: "food_references",
            type: "character varying(100)",
            maxLength: 100,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "character varying(200)",
            oldMaxLength: 200,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "barcode",
            table: "food_references",
            type: "character varying(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "character varying(100)",
            oldMaxLength: 100,
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "open_food_facts_id",
            table: "food_references",
            type: "character varying(100)",
            maxLength: 100,
            nullable: true);
    }
}
