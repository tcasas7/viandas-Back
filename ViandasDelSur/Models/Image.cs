namespace ViandasDelSur.Models
{
    public class Image
    {
        public long Id { get; set; }
        public string name { get; set; }
        public string route { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
