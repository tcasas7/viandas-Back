namespace ViandasDelSur.Models.DTOS
{
    public class DeliveryDTO
    {
        public int Id { get; set; }
        public int productId { get; set; }
        public bool delivered { get; set; }
        public DayOfWeek deliveryDate { get; set; }  // Usamos directamente DayOfWeek
        public int quantity { get; set; }
        public int MenuId { get; set; }

        public DeliveryDTO() { }

        public DeliveryDTO(Delivery delivery)
        {
            Id = delivery.Id;
            productId = delivery.productId;
            delivered = delivery.delivered;
            deliveryDate = delivery.deliveryDate.DayOfWeek;  // No se necesita conversión a int
            quantity = delivery.quantity;
        }
    }
}





