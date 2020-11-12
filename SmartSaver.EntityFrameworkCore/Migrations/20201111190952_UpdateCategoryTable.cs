using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartSaver.EntityFrameworkCore.Migrations
{
    public partial class UpdateCategoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TypeOfIncome",
                table: "Categories",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TypeOfIncome",
                table: "Categories");
        }
    }
}
