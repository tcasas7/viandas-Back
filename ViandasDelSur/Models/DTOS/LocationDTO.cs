namespace ViandasDelSur.Models.DTOS
{
    public class LocationDTO
    {
        public int Id { get; set; }
        public string dir { get; set; }
        public bool isDefault { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public LocationDTO() { }

        public LocationDTO(Location location)
        {
            Id = location.Id;
            dir = location.dir;
            isDefault = location.isDefault;
            Latitude = location.Latitude;
            Longitude = location.Longitude;
        }
    }
}
