using ViandasDelSur.Models;
using ViandasDelSur.Models.Enums;
using ViandasDelSur.Models.Responses;
using ViandasDelSur.Services.Interfaces;

namespace ViandasDelSur.Services.Implementations
{
    public class VerificationService : IVerificationService
    {
        public Response VerifyAdmin(User user)
        {
            Response response = new Response();

            if (user == null)
            {
                response.statusCode = 401;
                response.message = "Sesion invalida";
                return response;
            }

            if (user.role != Role.ADMIN)
            {
                response.statusCode = 403;
                response.message = "Prohibido";
                return response;
            }

            response.statusCode = 200;
            response.message = "Ok";
            return response;
        }

        public Response VerifyDelivery(User user)
        {
            Response response = new Response();

            if (user == null)
            {
                response.statusCode = 401;
                response.message = "Sesion invalida";
                return response;
            }

            if (user.role != Role.DELIVERY)
            {
                response.statusCode = 403;
                response.message = "Prohibido";
                return response;
            }

            response.statusCode = 200;
            response.message = "Ok";
            return response;
        }
    }
}
