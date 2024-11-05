using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Repositories.Interfaces;

namespace ViandasDelSur.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public string category { get; set; }
        public DateTime validDate { get; set; }
        public decimal price { get; set; }
        public bool IsDeleted { get; set; } = false;
        

        public ICollection<Product> Products { get; set; }

        public Menu() { }

        public Menu(MenuDTO menuDTO, IImageRepository imageRepository)
        {
            Id = menuDTO.Id;
            category = menuDTO.category;
            validDate = menuDTO.validDate;
            price = menuDTO.price;
           
            Products = new List<Product>();

            foreach (var productDTO in menuDTO.products)
            {
                // Obtener la imagen del repositorio usando el imageId del productDTO
                var image = imageRepository.GetById(productDTO.imageId);

                if (image != null)
                {
                    Products.Add(new Product(productDTO, image));
                }
                else
                {
                    // Si no se encuentra la imagen, se puede usar una imagen predeterminada
                    var defaultImage = imageRepository.GetByName("Default");
                    if (defaultImage != null)
                    {
                        Products.Add(new Product(productDTO, defaultImage));
                    }
                    else
                    {
                        throw new Exception("No se encontró una imagen para el producto con Id " + productDTO.Id);
                    }
                }
            }
        }
    }
}



