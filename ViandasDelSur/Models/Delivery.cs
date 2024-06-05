namespace ViandasDelSur.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        public bool delivered { get; set; }
        public DateTime deliveryDate { get; set; }

        public int orderId { get; set; }
        public Order Order { get; set; }

        public int productId { get; set; }
        public Product Product { get; set; }
    }
}
