using ViandasDelSur.Models;
using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models.Responses;

namespace ViandasDelSur.Services.Interfaces
{
    public interface IUsersService
    {
        public Response GetAll(string email);
        public Response Register(RegisterDTO model);
        public Response ChangePassword(ChangePasswordDTO model);
        public Response ChangeRole(ChangeRoleDTO model, string adminEmail);
        public Response ChangePhone(string email, string phone);
        public Response Data(string email);
        public Response AddLocation(LocationDTO model, string email);
        public Response RemoveLocation(LocationDTO model, string email);
    }
}
