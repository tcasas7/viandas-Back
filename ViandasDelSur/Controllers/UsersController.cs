using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using ViandasDelSur.Models;
using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models.Enums;
using ViandasDelSur.Models.Responses;
using ViandasDelSur.Repositories.Interfaces;
using ViandasDelSur.Services.Interfaces;
using ViandasDelSur.Tools;

namespace ViandasDelSur.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IVerificationService _verificationService;
        private readonly Encrypter _encrypter;

        public UsersController(
            IUserRepository userRepository,
            ILocationRepository locationRepository,
            IVerificationService verificationService)
        {
            _userRepository = userRepository;
            _locationRepository = locationRepository;
            _verificationService = verificationService;
            _encrypter = new Encrypter();        
        }

        [Authorize]
        [HttpGet]
        public ActionResult<AnyType> GetAll()
        {
            Response response = new Response();

            try
            {
                string email = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                var user = _userRepository.FindByEmail(email);

                response = _verificationService.VerifyAdmin(user);

                var users = _userRepository.GetAllUsers().ToList();

                if (users == null || users.Count <= 0)
                {
                    response.statusCode = 404;
                    response.message = "Usuarios no encontrados";
                    return new JsonResult(response);
                }

                List<UserDTO> result = new List<UserDTO>();

                foreach (var item in users)
                {
                    UserDTO userDTO = new UserDTO(item);
                    result.Add(userDTO);
                }

                response = new ResponseCollection<UserDTO>(200, "Ok", result);

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

                var user = _userRepository.FindByEmail(model.email);

                if (user != null)
                {
                    response.statusCode = 401;
                    response.message = "El email ya esta en uso";
                    return new JsonResult(response);
                }
               
                _encrypter.EncryptString(model.password, out byte[] hash, out byte[] salt);

                User newUser = new User();

                newUser.email = model.email;
                newUser.firstName = model.firstName;
                newUser.lastName = model.lastName;
                newUser.phone = model.phone;
                newUser.hash = hash;
                newUser.salt = salt;
                newUser.role = Models.Enums.Role.CLIENT;

                _userRepository.Save(newUser);

                response.statusCode = 200;
                response.message = "Ok";
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
                var user = _userRepository.FindByEmail(model.email);

                if (user == null)
                {
                    response.statusCode = 404;
                    response.message = "Usuario no encontrado";
                    return new JsonResult(response);
                }

                if(user.phone != model.phone)
                {
                    response.statusCode = 404;
                    response.message = "Usuario no encontrado";
                    return new JsonResult(response);
                }

                _encrypter.EncryptString(model.password, out byte[] hash, out byte[] salt);

                user.hash = hash;
                user.salt = salt;

                _userRepository.Save(user);

                response.statusCode = 200;
                response.message = "Ok";
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
                string adminEmail = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                var admin = _userRepository.FindByEmail(adminEmail);

                response = _verificationService.VerifyAdmin(admin);

                if (response.statusCode != 200)
                {
                    response.statusCode = 403;
                    response.message = "Sesión invalida";
                    return new JsonResult(response);
                }

                var user = _userRepository.FindByEmail(model.email);

                if (user == null)
                {
                    response.statusCode = 404;
                    response.message = "Usuario no encontrado";
                    return new JsonResult(response);
                }

                user.role = (Role)Enum.Parse(typeof(Role), model.Role, true);

                _userRepository.Save(user);

                response.statusCode = 200;
                response.message = "Ok";
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

                var user = _userRepository.FindByEmail(email);

                if (user == null)
                {
                    response.statusCode = 401;
                    response.message = "Sesión invalida";
                    return new JsonResult(response);
                }

                UserDTO userDTO = new UserDTO(user);

                response = new ResponseModel<UserDTO>(200, "Ok", userDTO);

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
                string email = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                var user = _userRepository.FindByEmail(email);

                if (user == null)
                {
                    response.statusCode = 401;
                    response.message = "Sesión invalida";
                    return new JsonResult(response);
                }

                Location newLocation = new Location();

                newLocation.Id = model.Id;
                newLocation.dir = model.dir;
                newLocation.userId = user.Id;

                _locationRepository.Save(newLocation);

                response.statusCode = 200;
                response.message = "Ok";
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
                string email = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                var user = _userRepository.FindByEmail(email);

                if (user == null)
                {
                    response.statusCode = 401;
                    response.message = "Sesión invalida";
                    return new JsonResult(response);
                }

                var location = _locationRepository.FindById(model.Id);

                if (location == null || location.userId != user.Id)
                {
                    response.statusCode = 404;
                    response.message = "Dirección no encontrada";
                    return new JsonResult(response);
                }

                _locationRepository.Remove(location);

                response.statusCode = 200;
                response.message = "Ok";
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
