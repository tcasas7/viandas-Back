using ViandasDelSur.Models;

public class ProductDTO
{
    public int Id { get; set; }
    public DayOfWeek day { get; set; }  // Usamos directamente DayOfWeek
    public string name { get; set; }
    public string imageFileName { get; set; }
    public long imageId { get; set; }
    public decimal price { get; set; }

    public ProductDTO() { }

    public ProductDTO(Product product)
    {
        Id = product.Id;
        day = product.day;  // Sin conversiones, ya que ambos son DayOfWeek
        name = product.name;
        imageFileName = product.Image.name;
        imageId = product.imageId;
        price = product.price;
    }
}



