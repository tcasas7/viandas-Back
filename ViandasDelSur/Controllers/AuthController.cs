using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models.Responses;
using ViandasDelSur.Models;
using ViandasDelSur.Repositories.Interfaces;
using ViandasDelSur.Services.Interfaces;

namespace ViandasDelSur.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;

        public AuthController(IUserRepository userRepository, IAuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }
        [HttpPost("login")]
        public ActionResult<AnyType> Login([FromBody] LoginDTO model)
        {
            Response response = new Response();

            try
            {
                if (string.IsNullOrEmpty(model.email) || string.IsNullOrEmpty(model.password))
                {
                    response.statusCode = 401;
                    response.message = "Campos inválidos";
                    return new JsonResult(response);
                }

                User user = _userRepository.FindByEmail(model.email);

                if (user == null)
                {
                    response.statusCode = 401;
                    response.message = "Usuario no encontrado";
                    return new JsonResult(response);
                }

                // Verificar si la cuenta está aprobada
                if (!user.IsVerified)
                {
                    response.statusCode = 403; // Forbidden
                    response.message = "Cuenta pendiente de aprobación";
                    return new JsonResult(response);
                }

                response = _authService.Login(model, user);

                if (response.statusCode != 200)
                    return new JsonResult(response);

                string token = _authService.MakeToken(user.email, user.role.ToString(), 15);

                response = new ResponseModel<string>(200, "Ok", token);

                return new JsonResult(response);
            }
            catch (Exception e)
            {
                response.statusCode = 500;
                response.message = e.Message;

                return new JsonResult(response);
            }
        }

        [HttpGet("health")]
        public ActionResult<AnyType> Health()
        {
            Response response = new Response();
            try
            {
                response.statusCode = 200;
                response.message = "Servidor activo";
                return new JsonResult(response);
            }
            catch (Exception e)
            {
                response.statusCode = 500;
                response.message = e.Message;
                return new JsonResult(response);
            }
        }

        [Authorize]
        [HttpGet("renew")]
        public ActionResult<AnyType> Renew()
        {
            Response response = new Response();
            try
            {
                string email = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;
                User user = _userRepository.FindByEmail(email);

                if (user == null)
                {
                    response.statusCode = 401;
                    response.message = "Sesión invalida";

                    return new JsonResult(response);
                }

                string token = _authService.MakeToken(user.email, user.role.ToString(), 15);

                response = new ResponseModel<string>(200, "Ok", token);

                return new JsonResult(response);
            }
            catch (Exception e)
            {
                response.statusCode = 500;
                response.message = e.Message;

                return new JsonResult(response);
            }
        }
    }
}
