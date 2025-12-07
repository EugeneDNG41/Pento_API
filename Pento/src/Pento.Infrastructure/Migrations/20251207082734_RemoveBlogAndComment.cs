using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RemoveBlogAndComment : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "comments");

        migrationBuilder.DropTable(
            name: "blog_posts");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "blog_posts",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                post_type = table.Column<int>(type: "integer", nullable: false),
                title = table.Column<string>(type: "text", nullable: false),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_blog_posts", x => x.id);
                table.ForeignKey(
                    name: "fk_blog_posts_user_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "comments",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                blog_post_id = table.Column<Guid>(type: "uuid", nullable: false),
                content = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                is_moderated = table.Column<bool>(type: "boolean", nullable: false),
                moderated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_comments", x => x.id);
                table.ForeignKey(
                    name: "fk_comments_blog_posts_blog_post_id",
                    column: x => x.blog_post_id,
                    principalTable: "blog_posts",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_comments_user_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_blog_posts_user_id",
            table: "blog_posts",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_comments_blog_post_id",
            table: "comments",
            column: "blog_post_id");

        migrationBuilder.CreateIndex(
            name: "ix_comments_user_id",
            table: "comments",
            column: "user_id");
    }
}
