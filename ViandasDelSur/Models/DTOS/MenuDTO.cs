namespace ViandasDelSur.Models.DTOS
{
    public class MenuDTO
    {
        public int Id { get; set; }
        public string category { get; set; }
        public DateTime validDate { get; set; }
        public decimal price { get; set; }

        public List<ProductDTO> products { get; set; } = new List<ProductDTO>();

        public MenuDTO()
        {

        }
        public MenuDTO(Menu menu) 
        {
            Id = menu.Id;
            category = menu.category;
            validDate = menu.validDate;
            price = menu.price;

            foreach (var product in menu.Products)
            {
                ProductDTO productDTO = new ProductDTO(product);
                products.Add(productDTO);
            }
        }
    }
}
