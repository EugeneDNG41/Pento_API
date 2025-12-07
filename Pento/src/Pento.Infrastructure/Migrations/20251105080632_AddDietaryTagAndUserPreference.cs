using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddDietaryTagAndUserPreference : Migration
{
    private static readonly string[] columns = new[] { "food_reference_id", "dietary_tag_id" };
    private static readonly string[] columnsArray = new[] { "recipe_id", "dietary_tag_id" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "dietary_tags",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                is_archived = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_dietary_tags", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "food_dietary_tags",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                food_reference_id = table.Column<Guid>(type: "uuid", nullable: false),
                dietary_tag_id = table.Column<Guid>(type: "uuid", nullable: false),
                is_archived = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_food_dietary_tags", x => x.id);
                table.ForeignKey(
                    name: "fk_food_dietary_tags_dietary_tags_dietary_tag_id",
                    column: x => x.dietary_tag_id,
                    principalTable: "dietary_tags",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "recipe_dietary_tags",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                dietary_tag_id = table.Column<Guid>(type: "uuid", nullable: false),
                is_archived = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_recipe_dietary_tags", x => x.id);
                table.ForeignKey(
                    name: "fk_recipe_dietary_tags_dietary_tags_dietary_tag_id",
                    column: x => x.dietary_tag_id,
                    principalTable: "dietary_tags",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "user_preferences",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                dietary_tag_id = table.Column<Guid>(type: "uuid", nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_archived = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_preferences", x => x.id);
                table.ForeignKey(
                    name: "fk_user_preferences_dietary_tags_dietary_tag_id",
                    column: x => x.dietary_tag_id,
                    principalTable: "dietary_tags",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_food_dietary_tags_dietary_tag_id",
            table: "food_dietary_tags",
            column: "dietary_tag_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_dietary_tags_food_reference_id_dietary_tag_id",
            table: "food_dietary_tags",
            columns: columns,
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_recipe_dietary_tags_dietary_tag_id",
            table: "recipe_dietary_tags",
            column: "dietary_tag_id");

        migrationBuilder.CreateIndex(
            name: "ix_recipe_dietary_tags_recipe_id_dietary_tag_id",
            table: "recipe_dietary_tags",
            columns: columnsArray,
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_user_preferences_dietary_tag_id",
            table: "user_preferences",
            column: "dietary_tag_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_preferences_user_id_dietary_tag_id",
            table: "user_preferences",
            columns: ["user_id", "dietary_tag_id"],
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "food_dietary_tags");

        migrationBuilder.DropTable(
            name: "recipe_dietary_tags");

        migrationBuilder.DropTable(
            name: "user_preferences");

        migrationBuilder.DropTable(
            name: "dietary_tags");
    }
}
