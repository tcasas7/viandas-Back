using ViandasDelSur.Models;
using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models.Responses;

namespace ViandasDelSur.Services.Interfaces
{
    public interface IAuthService
    {
        public Response Login(LoginDTO model, User user);
        public string MakeToken(string email, string role, int minutes);
    }
}
