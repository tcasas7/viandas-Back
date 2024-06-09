using ViandasDelSur.Models.DTOS;

namespace ViandasDelSur.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public string category { get; set; }
        public DateTime validDate { get; set; }

        public ICollection<Product> Products { get; set; }

        public Menu(){}

        public Menu(MenuDTO menuDTO)
        {
            Id = menuDTO.Id;
            category = menuDTO.category;
            validDate = menuDTO.validDate;

            foreach (var productDTO in menuDTO.products)
            {
                Product product = new Product(productDTO);
                Products.Add(product);
            }
        }
    }
}
