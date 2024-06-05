namespace ViandasDelSur.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public string category { get; set; }
        public DateTime validDate { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
