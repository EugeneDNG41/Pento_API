using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class FeatureUpdate : Migration
{
    private static readonly string[] columns = new[] { "code", "default_quota", "default_reset_period", "description", "name" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_subscription_features_features_feature_name",
            table: "subscription_features");

        migrationBuilder.DropForeignKey(
            name: "fk_user_entitlements_features_feature_name",
            table: "user_entitlements");

        migrationBuilder.DropPrimaryKey(
            name: "pk_features",
            table: "features");

        migrationBuilder.DeleteData(
            table: "features",
            keyColumn: "name",
            keyValue: "AI Chef");

        migrationBuilder.DeleteData(
            table: "features",
            keyColumn: "name",
            keyValue: "Image Scanning");

        migrationBuilder.DeleteData(
            table: "features",
            keyColumn: "name",
            keyValue: "Meal Plan Slot");

        migrationBuilder.DeleteData(
            table: "features",
            keyColumn: "name",
            keyValue: "Receipt Scanning");

        migrationBuilder.DeleteData(
            table: "features",
            keyColumn: "name",
            keyValue: "Storage Slot");

        migrationBuilder.DropColumn(
            name: "entitlement_reset_per",
            table: "user_entitlements");

        migrationBuilder.DropColumn(
            name: "entitlement_reset_per",
            table: "subscription_features");

        migrationBuilder.RenameColumn(
            name: "feature_name",
            table: "user_entitlements",
            newName: "feature_code");

        migrationBuilder.RenameColumn(
            name: "entitlement_quota",
            table: "user_entitlements",
            newName: "quota");

        migrationBuilder.RenameIndex(
            name: "ix_user_entitlements_feature_name",
            table: "user_entitlements",
            newName: "ix_user_entitlements_feature_code");

        migrationBuilder.RenameColumn(
            name: "feature_name",
            table: "subscription_features",
            newName: "feature_code");

        migrationBuilder.RenameColumn(
            name: "entitlement_quota",
            table: "subscription_features",
            newName: "quota");

        migrationBuilder.RenameIndex(
            name: "ix_subscription_features_feature_name",
            table: "subscription_features",
            newName: "ix_subscription_features_feature_code");

        migrationBuilder.AlterColumn<string>(
            name: "feature_code",
            table: "user_entitlements",
            type: "character varying(50)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(100)");

        migrationBuilder.AddColumn<string>(
            name: "reset_period",
            table: "user_entitlements",
            type: "character varying(10)",
            maxLength: 10,
            nullable: true);

        migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "user_entitlements",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        migrationBuilder.AddColumn<bool>(
            name: "is_active",
            table: "subscriptions",
            type: "boolean",
            nullable: false,
            defaultValue: true);

        migrationBuilder.AlterColumn<string>(
            name: "feature_code",
            table: "subscription_features",
            type: "character varying(50)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(100)");

        migrationBuilder.AddColumn<string>(
            name: "reset_period",
            table: "subscription_features",
            type: "character varying(10)",
            maxLength: 10,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "code",
            table: "features",
            type: "character varying(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<int>(
            name: "default_quota",
            table: "features",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "default_reset_period",
            table: "features",
            type: "character varying(10)",
            maxLength: 10,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "description",
            table: "features",
            type: "character varying(500)",
            maxLength: 500,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddPrimaryKey(
            name: "pk_features",
            table: "features",
            column: "code");

        migrationBuilder.InsertData(
            table: "features",
            columns: columns,
            values: new object[,]
            {
                { "AI_CHEF", null, null, "Generate personalized recipes.", "AI Chef" },
                { "IMAGE_RECOGNITION", 5, "Day", "Detect food items from images.", "Image Scanning" },
                { "MEAL_PLAN_SLOT", 5, null, "Total meal plan slot for scheduling and tracking meals.", "Meal Plan Slot" },
                { "OCR", 5, "Day", "Automatically extract food item names from photographed receipts.", "Receipt Scanning" },
                { "STORAGE_SLOT", 5, null, "Total storage slots for pantry management.", "Storage Slot" }
            });

        migrationBuilder.AddForeignKey(
            name: "fk_subscription_features_features_feature_code",
            table: "subscription_features",
            column: "feature_code",
            principalTable: "features",
            principalColumn: "code",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_user_entitlements_features_feature_code",
            table: "user_entitlements",
            column: "feature_code",
            principalTable: "features",
            principalColumn: "code",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_subscription_features_features_feature_code",
            table: "subscription_features");

        migrationBuilder.DropForeignKey(
            name: "fk_user_entitlements_features_feature_code",
            table: "user_entitlements");

        migrationBuilder.DropPrimaryKey(
            name: "pk_features",
            table: "features");

        migrationBuilder.DeleteData(
            table: "features",
            keyColumn: "code",
            keyColumnType: "character varying(50)",
            keyValue: "AI_CHEF");

        migrationBuilder.DeleteData(
            table: "features",
            keyColumn: "code",
            keyColumnType: "character varying(50)",
            keyValue: "IMAGE_RECOGNITION");

        migrationBuilder.DeleteData(
            table: "features",
            keyColumn: "code",
            keyColumnType: "character varying(50)",
            keyValue: "MEAL_PLAN_SLOT");

        migrationBuilder.DeleteData(
            table: "features",
            keyColumn: "code",
            keyColumnType: "character varying(50)",
            keyValue: "OCR");

        migrationBuilder.DeleteData(
            table: "features",
            keyColumn: "code",
            keyColumnType: "character varying(50)",
            keyValue: "STORAGE_SLOT");

        migrationBuilder.DropColumn(
            name: "reset_period",
            table: "user_entitlements");

        migrationBuilder.DropColumn(
            name: "xmin",
            table: "user_entitlements");

        migrationBuilder.DropColumn(
            name: "is_active",
            table: "subscriptions");

        migrationBuilder.DropColumn(
            name: "reset_period",
            table: "subscription_features");

        migrationBuilder.DropColumn(
            name: "code",
            table: "features");

        migrationBuilder.DropColumn(
            name: "default_quota",
            table: "features");

        migrationBuilder.DropColumn(
            name: "default_reset_period",
            table: "features");

        migrationBuilder.DropColumn(
            name: "description",
            table: "features");

        migrationBuilder.RenameColumn(
            name: "feature_code",
            table: "user_entitlements",
            newName: "feature_name");

        migrationBuilder.RenameColumn(
            name: "quota",
            table: "user_entitlements",
            newName: "entitlement_quota");

        migrationBuilder.RenameIndex(
            name: "ix_user_entitlements_feature_code",
            table: "user_entitlements",
            newName: "ix_user_entitlements_feature_name");

        migrationBuilder.RenameColumn(
            name: "feature_code",
            table: "subscription_features",
            newName: "feature_name");

        migrationBuilder.RenameColumn(
            name: "quota",
            table: "subscription_features",
            newName: "entitlement_quota");

        migrationBuilder.RenameIndex(
            name: "ix_subscription_features_feature_code",
            table: "subscription_features",
            newName: "ix_subscription_features_feature_name");

        migrationBuilder.AlterColumn<string>(
            name: "feature_name",
            table: "user_entitlements",
            type: "character varying(100)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(50)");

        migrationBuilder.AddColumn<string>(
            name: "entitlement_reset_per",
            table: "user_entitlements",
            type: "character varying(50)",
            maxLength: 50,
            nullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "feature_name",
            table: "subscription_features",
            type: "character varying(100)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(50)");

        migrationBuilder.AddColumn<string>(
            name: "entitlement_reset_per",
            table: "subscription_features",
            type: "character varying(50)",
            maxLength: 50,
            nullable: true);

        migrationBuilder.AddPrimaryKey(
            name: "pk_features",
            table: "features",
            column: "name");

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
}
