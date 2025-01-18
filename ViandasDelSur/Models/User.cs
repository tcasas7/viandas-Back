using ViandasDelSur.Models.Enums;

namespace ViandasDelSur.Models
{
    public class User
    {
        public int Id { get; set; }
        public string email { get; set; }
        public Role role { get; set; }
        public byte[] salt { get; set; }
        public byte[] hash { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phone { get; set; }
        public bool IsVerified { get; set; } = false;

        public ICollection<Location> Locations { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
