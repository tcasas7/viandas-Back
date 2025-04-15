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
        public Response ChangePhone(ChangePhoneDTO model, string email);
        public Response Data(string email);
        Task<Response>AddLocation(LocationDTO model, string email);
        public Response MakeDefault(LocationDTO model, string email);
        public Response RemoveLocation(LocationDTO model, string email);
        public Response GetAllContacts(string email);
        public Response GetActiveContact(string email);
        public Response AddContact(ContactDTO model, string email);
        public Response UpdateContact(ContactDTO model, string email);
        public Response RemoveContact(ContactDTO model, string email);
        public Response MakeActive(ContactDTO model, string email);
        public Response GetPendingUsers();
        public Response ApproveUser(int userId);
        public void RejectUser(int userId);
        public User FindById(int userId);
        string GenerateResetToken(string email);
        void UpdatePasswordByEmail(string email, string newPassword);
        User GetUserByEmail(string email);


    }
}
