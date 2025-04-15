using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models.Responses;
using ViandasDelSur.Services.Interfaces;
using ViandasDelSur.Services.Implementations;
using System.Configuration;


namespace ViandasDelSur.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public UsersController(IUsersService usersService, IEmailService emailService, IConfiguration configuration)
        {
            _usersService = usersService;
            _emailService = emailService;
            _configuration = configuration;
        }

        [Authorize]
        [HttpGet]
        public ActionResult<AnyType> GetAll()
        {
            Response response = new Response();

            try
            {
                string email = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                response = _usersService.GetAll(email);

                return new JsonResult(response);
            }
            catch (Exception e)
            {
                response.statusCode = 500;
                response.message = e.Message;
                return new JsonResult(response);
            }
        }

        [HttpPost("register")]
        public ActionResult<AnyType> Register([FromBody] RegisterDTO model)
        {
            Response response = new Response();
            try
            {
                if (String.IsNullOrEmpty(model.email) ||
                    String.IsNullOrEmpty(model.password) ||
                    String.IsNullOrEmpty(model.firstName) ||
                    String.IsNullOrEmpty(model.lastName))
                {
                    response.statusCode = 400;
                    response.message = "Datos invalidos";
                    return new JsonResult(response);
                }

                response = _usersService.Register(model);

                return new JsonResult(response);
            }
            catch (Exception e)
            {
                response.statusCode = 500;
                response.message = e.Message;
                return new JsonResult(response);
            }
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordDTO model)
        {
            var user = _usersService.GetUserByEmail(model.email);
            if (user == null)
                return BadRequest(new { message = "No se encontró el email" });

            var token = _usersService.GenerateResetToken(model.email);

            var emailService = new EmailService();
            emailService.SendResetPasswordEmail(model.email, token);

            return Ok(new { message = "Correo enviado con instrucciones para restablecer la contraseña" });
        }



        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordDTO model)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            try
            {
                var principal = tokenHandler.ValidateToken(model.Token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var email = principal.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Token inválido.");

                _usersService.UpdatePasswordByEmail(email, model.NewPassword);

                return Ok(new { message = "Contraseña actualizada correctamente." });
            }
            catch
            {
                return BadRequest("Token inválido o expirado.");
            }
        }


        [HttpPost("changePassword")]
        public ActionResult<AnyType> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            Response response = new Response();

            try
            {
                if (String.IsNullOrEmpty(model.email) ||
                    String.IsNullOrEmpty(model.password) ||
                    String.IsNullOrEmpty(model.phone)
                    )
                {
                    response.statusCode = 400;
                    response.message = "Datos invalidos";
                    return new JsonResult(response);
                }

                response = _usersService.ChangePassword(model);

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
        [HttpPost("changeRole")]
        public ActionResult<AnyType> ChangeRole([FromBody] ChangeRoleDTO model)
        {
            Response response = new Response();

            try
            {
                if (String.IsNullOrEmpty(model.email) ||
                    String.IsNullOrEmpty(model.Role))
                {
                    response.statusCode = 400;
                    response.message = "Datos invalidos";
                    return new JsonResult(response);
                }

                string adminEmail = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                response = _usersService.ChangeRole(model, adminEmail);

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
        [HttpPost("changePhone")]
        public ActionResult<AnyType> ChangePhone([FromBody] ChangePhoneDTO model)
        {
            Response response = new Response();
            try
            {
                string email = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                response = _usersService.ChangePhone(model, email);

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
        [HttpGet("data")]
        public ActionResult<AnyType> Data()
        {
            Response response = new Response();

            try
            {
                string email = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                response = _usersService.Data(email);

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
        [HttpPost("addLocation")]
        public async Task<ActionResult<Response>> AddLocation([FromBody] LocationDTO model)
        {
            var response = new Response();
            try
            {
                // Validar datos básicos
                if (string.IsNullOrEmpty(model.dir) || model.Latitude == 0 || model.Longitude == 0)
                {
                    response.statusCode = 400;
                    response.message = "Datos inválidos. Por favor, complete todos los campos.";
                    return BadRequest(response);
                }

                string email = User.FindFirst("Account")?.Value ?? string.Empty;

                if (string.IsNullOrEmpty(email))
                {
                    response.statusCode = 401;
                    response.message = "Usuario no autorizado.";
                    return Unauthorized(response);
                }

                // Llamar al servicio para agregar la ubicación
                response = await _usersService.AddLocation(model, email);

                return new JsonResult(response);
            }
            catch (ArgumentOutOfRangeException e)
            {
                response.statusCode = 400;
                response.message = "Error al agregar la ubicación: " + e.Message;
                return BadRequest(response);
            }
            catch (Exception e)
            {
                response.statusCode = 500;
                response.message = "Error interno del servidor: " + e.Message;
                return StatusCode(500, response);
            }
        }


        [Authorize]
        [HttpPost("makeDefault")]
        public ActionResult<AnyType> MakeDefault([FromBody] LocationDTO model)
        {
            Response response = new Response();
            try
            {
                if (String.IsNullOrEmpty(model.dir))
                {
                    response.statusCode = 400;
                    response.message = "Datos invalidos";
                    return new JsonResult(response);
                }

                string email = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                response = _usersService.MakeDefault(model, email);

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
        [HttpPost("removeLocation")]
        public ActionResult<Response> RemoveLocation([FromBody] LocationDTO model)
        {
            var response = new Response();

            try
            {
                // Validar entrada
                if (model == null || string.IsNullOrEmpty(model.dir))
                {
                    response.statusCode = 400;
                    response.message = "Datos inválidos. Por favor, complete todos los campos.";
                    return BadRequest(response); // Retorna 400 Bad Request
                }

                // Obtener email del usuario autenticado
                string email = User.FindFirst("Account")?.Value ?? string.Empty;

                if (string.IsNullOrEmpty(email))
                {
                    response.statusCode = 401;
                    response.message = "No se pudo autenticar al usuario.";
                    return Unauthorized(response); // Retorna 401 Unauthorized
                }

                // Llamar al servicio para eliminar la ubicación
                response = _usersService.RemoveLocation(model, email);

                if (response.statusCode == 200)
                {
                    return Ok(response); // Retorna 200 OK si todo salió bien
                }
                else
                {
                    return BadRequest(response); // Retorna 400 si hay un problema conocido
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones no controladas
                response.statusCode = 500;
                response.message = $"Error interno del servidor: {ex.Message}";
                return StatusCode(500, response); // Retorna 500 Internal Server Error
            }
        }


        [Authorize]
        [HttpPost("addContact")]
        public ActionResult<Response> AddContact([FromBody] ContactDTO model)
        {
            var response = new Response();

            try
            {
                string email = User.FindFirst("Account")?.Value ?? string.Empty;

                // Verificar si el email se obtiene correctamente
                if (string.IsNullOrEmpty(email))
                {
                    response.statusCode = 401;
                    response.message = "No se pudo obtener el email del usuario.";
                    return new JsonResult(response);
                }

                // Intentar agregar el contacto
                response = _usersService.AddContact(model, email);

                if (response.statusCode != 200)
                {
                    return new JsonResult(response);
                }

                return new JsonResult(response);
            }
            catch (Exception e)
            {
                // Incluir más información sobre el error
                response.statusCode = 500;
                response.message = $"Error al agregar el contacto: {e.Message}";
                return new JsonResult(response);
            }
        }


        [Authorize]
        [HttpGet("getContacts")]
        public ActionResult<AnyType> GetAllContacts()
        {
            Response response = new Response();
            try
            {
                string email = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                response = _usersService.GetAllContacts(email);

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
        [HttpGet("getActiveContact")]
        public ActionResult<AnyType> GetActiveContact()
        {
            Response response = new Response();
            try
            {
                string email = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                response = _usersService.GetActiveContact(email);

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
        [HttpPost("updateContact")]
        public ActionResult<AnyType> UpdateContact([FromBody] ContactDTO model)
        {
            Response response = new Response();
            try
            {
                string email = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                response = _usersService.UpdateContact(model, email);

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
        [HttpPost("removeContact")]
        public ActionResult<AnyType> RemoveContact([FromBody] ContactDTO model)
        {
            Response response = new Response();
            try
            {
                string email = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                response = _usersService.RemoveContact(model, email);

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
        [HttpPost("makeActive")]
        public ActionResult<AnyType> MakeActive([FromBody] ContactDTO model)
        {
            Response response = new Response();
            try
            {
                string email = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                response = _usersService.MakeActive(model, email);

                return new JsonResult(response);
            }
            catch (Exception e)
            {
                response.statusCode = 500;
                response.message = e.Message;
                return new JsonResult(response);
            }
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("pendingUsers")]
        public ActionResult<AnyType> GetPendingUsers()
        {
            Response response = new Response();

            try
            {
                response = _usersService.GetPendingUsers();

                return new JsonResult(response);
            }
            catch (Exception e)
            {
                response.statusCode = 500;
                response.message = e.Message;
                return new JsonResult(response);
            }
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("approveUser/{id}")]
        public ActionResult<AnyType> ApproveUser(int id)
        {
            Response response = new Response();

            try
            {
                response = _usersService.ApproveUser(id);

                return new JsonResult(response);
            }
            catch (Exception e)
            {
                response.statusCode = 500;
                response.message = e.Message;
                return new JsonResult(response);
            }
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost("rejectUser")]
        public ActionResult<Response> RejectUser([FromBody] int userId)
        {
            try
            {
                var user = _usersService.FindById(userId);

                if (user == null)
                {
                    return NotFound(new Response
                    {
                        statusCode = 404,
                        message = "Usuario no encontrado."
                    });
                }

                _usersService.RejectUser(userId);

                return Ok(new Response
                {
                    statusCode = 200,
                    message = "Usuario rechazado exitosamente."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response
                {
                    statusCode = 500,
                    message = "Ocurrió un error: " + ex.Message
                });
            }
        }



    }
}
