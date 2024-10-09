using ViandasDelSur.Models.Enums;

namespace ViandasDelSur.Models.DTOS
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public decimal price { get; set; }
        public PaymentMethod paymentMethod { get; set; }
        public bool hasSalt { get; set; }
        public string description { get; set; }
        public DateTime orderDate { get; set; }
        public string location { get; set; }

        public List<DeliveryDTO> deliveries { get; set; } = new List<DeliveryDTO>();

        public OrderDTO()
        {
            
        }

        public OrderDTO(Order order)
        {
            Id = order.Id;
            price = order.price;
            paymentMethod = order.paymentMethod;
            hasSalt = order.hasSalt;
            description = order.description;
            orderDate = order.orderDate;
            location = order.location;

            foreach (var delivery in order.Deliveries)
            {
                DeliveryDTO deliveryDTO = new DeliveryDTO(delivery);
                deliveries.Add(deliveryDTO);
            }
        }
    }
}
