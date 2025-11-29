using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffIo.Data.Migrations
{
    /// <inheritdoc />
    public partial class newColumnValueInHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "Histories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "Histories");
        }
    }
}
