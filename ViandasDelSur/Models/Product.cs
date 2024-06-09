using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models.Enums;

namespace ViandasDelSur.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string name { get; set; }
        public DayOfWeek day { get; set; }

        public ICollection<Delivery> Deliveries { get; set; }

        public int menuId { get; set; }
        public Menu Menu { get; set; }

        public Product() { }

        public Product(ProductDTO productDTO)
        {
            Id = productDTO.Id;
            name = productDTO.name;
            day = productDTO.day;

            Deliveries = new List<Delivery>();
        }
    }
}
