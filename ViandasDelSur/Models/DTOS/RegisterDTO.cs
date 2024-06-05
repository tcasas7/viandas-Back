using System.Drawing;

namespace ViandasDelSur.Models.DTOS
{
    public class RegisterDTO
    {
        public string email { get; set; }
        public string password { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phone { get; set; }
    }
}
