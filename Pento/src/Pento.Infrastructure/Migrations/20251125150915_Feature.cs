using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class Feature : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "feature",
            table: "user_entitlements",
            newName: "feature_name");

        migrationBuilder.RenameColumn(
            name: "limit_reset_per",
            table: "user_entitlements",
            newName: "entitlement_reset_per");

        migrationBuilder.RenameColumn(
            name: "limit_quota",
            table: "user_entitlements",
            newName: "entitlement_quota");

        migrationBuilder.RenameColumn(
            name: "feature",
            table: "subscription_features",
            newName: "feature_name");

        migrationBuilder.RenameColumn(
            name: "limit_reset_per",
            table: "subscription_features",
            newName: "entitlement_reset_per");

        migrationBuilder.RenameColumn(
            name: "limit_quota",
            table: "subscription_features",
            newName: "entitlement_quota");

        migrationBuilder.AlterColumn<string>(
            name: "feature_name",
            table: "user_entitlements",
            type: "character varying(100)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(50)",
            oldMaxLength: 50);

        migrationBuilder.AlterColumn<string>(
            name: "feature_name",
            table: "subscription_features",
            type: "character varying(100)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(50)",
            oldMaxLength: 50);

        migrationBuilder.CreateTable(
            name: "features",
            columns: table => new
            {
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_features", x => x.name);
            });

        migrationBuilder.InsertData(
            table: "features",
            column: "name",
            values: new object[]
            {
                "AI Chef",
                "Image Scanning",
                "Meal Plan Slot",
                "Receipt Scanning",
                "Storage Slot"
            });

        migrationBuilder.CreateIndex(
            name: "ix_user_entitlements_feature_name",
            table: "user_entitlements",
            column: "feature_name");

        migrationBuilder.CreateIndex(
            name: "ix_subscription_features_feature_name",
            table: "subscription_features",
            column: "feature_name");

        migrationBuilder.AddForeignKey(
            name: "fk_subscription_features_features_feature_name",
            table: "subscription_features",
            column: "feature_name",
            principalTable: "features",
            principalColumn: "name",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_user_entitlements_features_feature_name",
            table: "user_entitlements",
            column: "feature_name",
            principalTable: "features",
            principalColumn: "name",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_subscription_features_features_feature_name",
            table: "subscription_features");

        migrationBuilder.DropForeignKey(
            name: "fk_user_entitlements_features_feature_name",
            table: "user_entitlements");

        migrationBuilder.DropTable(
            name: "features");

        migrationBuilder.DropIndex(
            name: "ix_user_entitlements_feature_name",
            table: "user_entitlements");

        migrationBuilder.DropIndex(
            name: "ix_subscription_features_feature_name",
            table: "subscription_features");

        migrationBuilder.RenameColumn(
            name: "feature_name",
            table: "user_entitlements",
            newName: "feature");

        migrationBuilder.RenameColumn(
            name: "entitlement_reset_per",
            table: "user_entitlements",
            newName: "limit_reset_per");

        migrationBuilder.RenameColumn(
            name: "entitlement_quota",
            table: "user_entitlements",
            newName: "limit_quota");

        migrationBuilder.RenameColumn(
            name: "feature_name",
            table: "subscription_features",
            newName: "feature");

        migrationBuilder.RenameColumn(
            name: "entitlement_reset_per",
            table: "subscription_features",
            newName: "limit_reset_per");

        migrationBuilder.RenameColumn(
            name: "entitlement_quota",
            table: "subscription_features",
            newName: "limit_quota");

        migrationBuilder.AlterColumn<string>(
            name: "feature",
            table: "user_entitlements",
            type: "character varying(50)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(100)");

        migrationBuilder.AlterColumn<string>(
            name: "feature",
            table: "subscription_features",
            type: "character varying(50)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(100)");
    }
}
