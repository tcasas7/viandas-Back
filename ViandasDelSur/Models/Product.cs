using ViandasDelSur.Models;

public class Product
{
    public int Id { get; set; }
    public string name { get; set; }
    public DayOfWeek day { get; set; }  // Aquí también usamos DayOfWeek
    public ICollection<Delivery> Deliveries { get; set; }
    public int menuId { get; set; }
    public Menu Menu { get; set; }
    public Image Image { get; set; }
    public long imageId { get; set; }
    public decimal price { get; set; }

    public Product() { }

    public Product(ProductDTO productDTO, Image img)
    {
        Id = productDTO.Id;
        name = productDTO.name;
        day = productDTO.day;  // No necesitamos conversiones
        Image = img;
        Deliveries = new List<Delivery>();
        price = productDTO.price;
    }
}

