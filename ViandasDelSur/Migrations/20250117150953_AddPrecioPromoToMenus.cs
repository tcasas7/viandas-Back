using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViandasDelSur.Migrations
{
    /// <inheritdoc />
    public partial class AddPrecioPromoToMenus : Migration
    {
        

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "precioPromo",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "precioPromo",
                table: "Menus");
        }
    }
}
