using ViandasDelSur.Models;

public class ProductDTO
{
    public int Id { get; set; }
    public DayOfWeek day { get; set; }  // Usamos directamente DayOfWeek
    public string name { get; set; }
    public string imageName { get; set; }
    public string imagePath { get; set; }
    public long imageId { get; set; }

    public ProductDTO() { }

    public ProductDTO(Product product)
    {
        Id = product.Id;
        day = product.day;  // Sin conversiones, ya que ambos son DayOfWeek
        name = product.name;
        imageName = product.Image.name;
        imagePath = $"/api/Menus/image/{product.Id}";
        imageId = product.imageId;
    }
}



