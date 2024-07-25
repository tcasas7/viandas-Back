namespace ViandasDelSur.Models.DTOS
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public DayOfWeek day { get; set; }
        public string name { get; set; }
        public string imageName { get; set; }

        public ProductDTO()
        {

        }
        public ProductDTO(Product product) 
        {
            Id = product.Id;
            day = product.day;
            name = product.name;
            imageName = product.Image.name;
        }
    }
}
