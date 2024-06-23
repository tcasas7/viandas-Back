using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViandasDelSur.Migrations
{
    /// <inheritdoc />
    public partial class v6_minorChangesForBetterUX : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "price",
                table: "Menus",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "isDefault",
                table: "Locations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "price",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "isDefault",
                table: "Locations");
        }
    }
}
