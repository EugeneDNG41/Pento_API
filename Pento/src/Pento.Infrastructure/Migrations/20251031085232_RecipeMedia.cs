using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RecipeMedia : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "recipe_media",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                mime_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                is_archived = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_recipe_media", x => x.id);
                table.ForeignKey(
                    name: "fk_recipe_media_recipe_recipe_id",
                    column: x => x.recipe_id,
                    principalTable: "recipes",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_recipe_media_recipe_id",
            table: "recipe_media",
            column: "recipe_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "recipe_media");
    }
}
