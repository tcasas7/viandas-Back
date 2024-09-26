


namespace ViandasDelSur.Models.DTOS
{
    public class DeliveryDTO
    {
        public int Id { get; set; }
        public int productId { get; set; }
        public bool delivered { get; set; }
        public int deliveryDate { get; set; } // Cambiado a int
        public int quantity { get; set; }

        public DeliveryDTO() { }

        public DeliveryDTO(Delivery delivery)
        {
            Id = delivery.Id;
            productId = delivery.productId;
            delivered = delivery.delivered;
            deliveryDate = (int)delivery.deliveryDate.DayOfWeek; // Convertimos de DayOfWeek a int
            quantity = delivery.quantity;
        }
    }
}





/*namespace ViandasDelSur.Models.DTOS
{
    public class DeliveryDTO
    {
        public int Id { get; set; }
        public int productId { get; set; }
        public bool delivered { get; set; }
        public DayOfWeek deliveryDate { get; set; }
        public int quantity { get; set; }
        public DeliveryDTO() { }

        public DeliveryDTO(Delivery delivery)
        {
            Id = delivery.Id;
            productId = delivery.productId;
            delivered = delivery.delivered;
            deliveryDate = delivery.deliveryDate.DayOfWeek;
            quantity = delivery.quantity;
        }
    }
}*/
