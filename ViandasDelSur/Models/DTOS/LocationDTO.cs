namespace ViandasDelSur.Models.DTOS
{
    public class LocationDTO
    {
        public int Id { get; set; }
        public string dir { get; set; }

        public LocationDTO() { }

        public LocationDTO(Location location)
        {
            Id = location.Id;
            dir = location.dir;
        }
    }
}
