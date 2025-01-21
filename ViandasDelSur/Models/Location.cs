namespace ViandasDelSur.Models
{
    public class Location
    {
        public int Id { get; set; }
        public string dir { get; set; }
        public bool isDefault { get; set; }
        public int userId { get; set; }
        public User User { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

    }
}
