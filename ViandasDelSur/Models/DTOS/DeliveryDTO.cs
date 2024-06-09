namespace ViandasDelSur.Models.DTOS
{
    public class DeliveryDTO
    {
        public int Id { get; set; }
        public int productId { get; set; }
        public bool delivered { get; set; }
        public DayOfWeek deliveryDate { get; set; }
        public DeliveryDTO() { }

        public DeliveryDTO(Delivery delivery)
        {
            Id = delivery.Id;
            productId = delivery.productId;
            delivered = delivery.delivered;
            deliveryDate = delivery.deliveryDate.DayOfWeek;
        }
    }
}
