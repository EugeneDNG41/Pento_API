using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RecipeWishlistRefractor : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "is_deleted",
            table: "recipe_wishlists");

        migrationBuilder.AddColumn<Guid>(
            name: "user_id",
            table: "recipe_wishlists",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty);

        migrationBuilder.CreateIndex(
            name: "ix_recipe_wishlists_user_id",
            table: "recipe_wishlists",
            column: "user_id");

        migrationBuilder.AddForeignKey(
            name: "fk_recipe_wishlists_user_user_id",
            table: "recipe_wishlists",
            column: "user_id",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_recipe_wishlists_user_user_id",
            table: "recipe_wishlists");

        migrationBuilder.DropIndex(
            name: "ix_recipe_wishlists_user_id",
            table: "recipe_wishlists");

        migrationBuilder.DropColumn(
            name: "user_id",
            table: "recipe_wishlists");

        migrationBuilder.AddColumn<bool>(
            name: "is_deleted",
            table: "recipe_wishlists",
            type: "boolean",
            nullable: false,
            defaultValue: false);
    }
}
