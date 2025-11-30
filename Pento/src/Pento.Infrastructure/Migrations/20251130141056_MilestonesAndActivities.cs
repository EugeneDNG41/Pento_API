using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class MilestonesAndActivities : Migration
{
    private static readonly string[] columns = new[] { "code", "description", "name" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "activities",
            columns: table => new
            {
                code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_activities", x => x.code);
            });

        migrationBuilder.CreateTable(
            name: "milestones",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_milestones", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "user_activities",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                activity_code = table.Column<string>(type: "character varying(50)", nullable: false),
                performed_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_activities", x => x.id);
                table.ForeignKey(
                    name: "fk_user_activities_activities_activity_code",
                    column: x => x.activity_code,
                    principalTable: "activities",
                    principalColumn: "code",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "milestone_requirements",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                milestone_id = table.Column<Guid>(type: "uuid", nullable: false),
                activity_code = table.Column<string>(type: "character varying(50)", nullable: false),
                quota = table.Column<int>(type: "integer", nullable: false),
                within_days = table.Column<int>(type: "integer", nullable: true),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_milestone_requirements", x => x.id);
                table.ForeignKey(
                    name: "fk_milestone_requirements_activities_activity_code",
                    column: x => x.activity_code,
                    principalTable: "activities",
                    principalColumn: "code",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_milestone_requirements_milestones_milestone_id",
                    column: x => x.milestone_id,
                    principalTable: "milestones",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "user_milestones",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                milestone_id = table.Column<Guid>(type: "uuid", nullable: false),
                achieved_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_milestones", x => x.id);
                table.ForeignKey(
                    name: "fk_user_milestones_milestones_milestone_id",
                    column: x => x.milestone_id,
                    principalTable: "milestones",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.InsertData(
            table: "activities",
            columns: columns,
            values: new object[,]
            {
                { "FOOD_ITEM_CONSUME", "Consuming a food item from your storage/compartment.", "Consume Food Item" },
                { "HOUSEHOLD_CREATE", "Creating a new household to manage your food, grocery lists, and meal plans with others.", "Create Household" },
                { "STORAGE_CREATE", "Creating a new storage location to store your food items.", "Create Storage" }
            });

        migrationBuilder.CreateIndex(
            name: "ix_milestone_requirements_activity_code",
            table: "milestone_requirements",
            column: "activity_code");

        migrationBuilder.CreateIndex(
            name: "ix_milestone_requirements_milestone_id",
            table: "milestone_requirements",
            column: "milestone_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_activities_activity_code",
            table: "user_activities",
            column: "activity_code");

        migrationBuilder.CreateIndex(
            name: "ix_user_milestones_milestone_id",
            table: "user_milestones",
            column: "milestone_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "milestone_requirements");

        migrationBuilder.DropTable(
            name: "user_activities");

        migrationBuilder.DropTable(
            name: "user_milestones");

        migrationBuilder.DropTable(
            name: "activities");

        migrationBuilder.DropTable(
            name: "milestones");
    }
}
