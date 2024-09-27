namespace ViandasDelSur.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        public bool delivered { get; set; }
        public DateTime deliveryDate { get; set; }  // Mantiene la fecha completa
        public int quantity { get; set; }

        public int orderId { get; set; }
        public Order Order { get; set; }

        public int productId { get; set; }
        public Product Product { get; set; }

        // Propiedad de solo lectura para obtener el día de la semana
        public DayOfWeek DayOfWeek => deliveryDate.DayOfWeek;
    }
}
