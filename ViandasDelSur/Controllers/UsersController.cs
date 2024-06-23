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
        [HttpPost("changePhone/{phone}")]
        public ActionResult<AnyType> ChangePhone(string phone)
        {
            Response response = new Response();
            try
            {
                string email = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                response = _usersService.ChangePhone(email, phone);

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
        public ActionResult<AnyType> AddLocation([FromBody] LocationDTO model)
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

                response = _usersService.AddLocation(model, email);
                
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
    }
}
