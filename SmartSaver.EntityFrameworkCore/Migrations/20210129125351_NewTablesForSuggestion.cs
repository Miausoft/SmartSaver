using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartSaver.EntityFrameworkCore.Migrations
{
    public partial class NewTablesForSuggestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateCheckConstraint(
                name: "PositiveGoal",
                table: "Accounts",
                sql: "Goal >= 0");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateJoined",
                table: "Users",
                nullable: false,
                defaultValueSql: "getdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ActionTime",
                table: "Transactions",
                nullable: false,
                defaultValueSql: "getdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "EmailVerifications",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Categories",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Goal",
                table: "Accounts",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.CreateTable(
                name: "SolutionSuggestions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolutionText = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolutionSuggestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProblemSuggestions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProblemText = table.Column<string>(nullable: true),
                    SolutionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemSuggestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProblemSuggestions_SolutionSuggestions_SolutionId",
                        column: x => x.SolutionId,
                        principalTable: "SolutionSuggestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerifications_Token",
                table: "EmailVerifications",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProblemSuggestions_ProblemText",
                table: "ProblemSuggestions",
                column: "ProblemText",
                unique: true,
                filter: "[ProblemText] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemSuggestions_SolutionId",
                table: "ProblemSuggestions",
                column: "SolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_SolutionSuggestions_SolutionText",
                table: "SolutionSuggestions",
                column: "SolutionText",
                unique: true,
                filter: "[SolutionText] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProblemSuggestions");

            migrationBuilder.DropTable(
                name: "SolutionSuggestions");

            migrationBuilder.DropIndex(
                name: "IX_EmailVerifications_Token",
                table: "EmailVerifications");

            migrationBuilder.DropCheckConstraint(
                name: "PositiveGoal",
                table: "Accounts");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateJoined",
                table: "Users",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "getdate()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ActionTime",
                table: "Transactions",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "getdate()");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "EmailVerifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<decimal>(
                name: "Goal",
                table: "Accounts",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldDefaultValue: 0m);
        }
    }
}
