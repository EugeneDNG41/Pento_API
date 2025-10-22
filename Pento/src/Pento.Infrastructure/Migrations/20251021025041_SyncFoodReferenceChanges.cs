using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class SyncFoodReferenceChanges : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
        ALTER TABLE food_references
        ALTER COLUMN food_group TYPE character varying(50)
        USING food_group::text;
    ");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
        ALTER TABLE food_references
        ALTER COLUMN food_group TYPE integer
        USING food_group::integer;
    ");
    }
}
