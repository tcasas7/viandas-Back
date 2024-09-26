namespace ViandasDelSur.Models.DTOS
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public int day { get; set; }
        public string name { get; set; }
        public string imageName { get; set; }

        public ProductDTO()
        {

        }
        /*public ProductDTO(Product product) 
        {
            Id = product.Id;
            day = product.day;
            name = product.name;
            imageName = product.Image.name;
        }
    }*/

        public ProductDTO(Product product)
        {
            Id = product.Id;

            // Mapeo de DayOfWeek a números 1-5 (Lunes a Viernes)
            day = product.day switch
            {
                DayOfWeek.Monday => 1,
                DayOfWeek.Tuesday => 2,
                DayOfWeek.Wednesday => 3,
                DayOfWeek.Thursday => 4,
                DayOfWeek.Friday => 5,
                _ => 0 // Cualquier otro día se ignora (invalido)
            };

            name = product.name;
            imageName = product.Image.name;
        }
    }
}

    
