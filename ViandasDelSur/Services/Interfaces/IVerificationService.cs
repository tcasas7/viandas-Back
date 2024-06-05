using ViandasDelSur.Models;
using ViandasDelSur.Models.Responses;

namespace ViandasDelSur.Services.Interfaces
{
    public interface IVerificationService
    {
        public Response VerifyAdmin(User user);
        public Response VerifyDelivery(User user);
    }
}
