using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViandasDelSur.Migrations
{
    /// <inheritdoc />
    public partial class AddPrecioPromoToMenusAndOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Agregar columna precioPromo a la tabla Orders
            migrationBuilder.AddColumn<decimal>(
                name: "precioPromo",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: true);

            // Agregar columna precioPromo a la tabla Menus
            migrationBuilder.AddColumn<decimal>(
                name: "precioPromo",
                table: "Menus",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar columna precioPromo de la tabla Orders
            migrationBuilder.DropColumn(
                name: "precioPromo",
                table: "Orders");

            // Eliminar columna precioPromo de la tabla Menus
            migrationBuilder.DropColumn(
                name: "precioPromo",
                table: "Menus");
        }
    }
}
