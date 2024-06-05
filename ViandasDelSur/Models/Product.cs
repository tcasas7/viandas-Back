using ViandasDelSur.Models.Enums;

namespace ViandasDelSur.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string name { get; set; }
        public Day day { get; set; }

        public ICollection<Delivery> Deliveries { get; set; }

        public int menuId { get; set; }
        public Menu Menu { get; set; }
    }
}
