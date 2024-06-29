using ViandasDelSur.Models.Enums;

namespace ViandasDelSur.Models
{
    public class Order
    {
        public int Id { get; set; }
        public double price { get; set; }
        public PaymentMethod paymentMethod { get; set; }
        public bool hasSalt { get; set; }
        public string description { get; set; }
        public DateTime orderDate { get; set; }
        public string location { get; set; }

        public int userId { get; set; }
        public User User { get; set; }

        public ICollection<Delivery> Deliveries { get; set; }
    }
}
