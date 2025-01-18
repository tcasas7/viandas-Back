using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models.Responses;
using ViandasDelSur.Services.Interfaces;

namespace ViandasDelSur.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
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
        public async Task<ActionResult<AnyType>> AddLocation([FromBody] LocationDTO model)
        {
            Response response = new Response();
            try
            {
                if (String.IsNullOrEmpty(model.dir))
                {
                    response.statusCode = 400;
                    response.message = "Datos inválidos";
                    return new JsonResult(response);
                }

                string email = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                // Espera el resultado asincrónico del servicio
                response = await _usersService.AddLocation(model, email);

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
        public ActionResult<AnyType> RemoveLocation([FromBody] LocationDTO model)
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

                response = _usersService.RemoveLocation(model, email);

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
