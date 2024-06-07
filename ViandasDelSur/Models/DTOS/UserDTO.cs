using ViandasDelSur.Models.Enums;

namespace ViandasDelSur.Models.DTOS
{
    public class UserDTO
    {
        public int Id { get; set; }
        public Role role { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public List<LocationDTO> locations { get; set; } = new List<LocationDTO>();
        //agregar orders

        public UserDTO() { }

        public UserDTO(User user)
        {
            Id = user.Id;
            role = user.role;
            phone = user.phone;
            email = user.email;
            firstName = user.firstName;
            lastName = user.lastName;

            foreach (var location in user.Locations)
            {
                LocationDTO locationDTO = new LocationDTO(location);
                locations.Add(locationDTO);
            }
            //agregar orders
        }
    }
}
