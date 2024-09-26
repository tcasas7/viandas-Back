using ViandasDelSur.Models.DTOS;

namespace ViandasDelSur.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string name { get; set; }
        public DayOfWeek day { get; set; }  // Aquí usamos DayOfWeek

        public ICollection<Delivery> Deliveries { get; set; }

        public int menuId { get; set; }
        public Menu Menu { get; set; }

        public Image Image { get; set; }
        public long imageId { get; set; }

        public Product() { }

        public Product(ProductDTO productDTO)
        {
            name = productDTO.name;
            day = (DayOfWeek)(productDTO.day - 1);  // Conversión explícita de int a DayOfWeek

            Deliveries = new List<Delivery>();
        }

        public Product(ProductDTO productDTO, Image img)
        {
            Id = productDTO.Id;
            name = productDTO.name;
            day = (DayOfWeek)(productDTO.day - 1);  // Conversión explícita de int a DayOfWeek

            Image = img;
            Deliveries = new List<Delivery>();
        }
    }
}

