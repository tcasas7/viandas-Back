using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Newtonsoft.Json;
using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models.Enums;
using ViandasDelSur.Models.Responses;
using ViandasDelSur.Repositories.Implementations;
using ViandasDelSur.Repositories.Interfaces;
using ViandasDelSur.Services.Interfaces;

namespace ViandasDelSur.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _ordersService;
        private readonly IUserRepository _userRepository;

        public OrdersController(IOrdersService ordersService, IUserRepository userRepository)
        {
            _ordersService = ordersService;
            _userRepository = userRepository;
        }


        [Authorize]
        [HttpGet("getDates")]
        public ActionResult<AnyType> GetDates()
        {
            Response response = new Response();

            try
            {
                string adminEmail = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;
                response = _ordersService.GetDates(adminEmail);

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
        [HttpGet("getAll")]
        public ActionResult<AnyType> GetAllOrders()
        {
            Response response = new Response();

            try
            {
                
                string adminEmail = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;
                var user = _userRepository.FindByEmail(adminEmail);

               
                if (user == null || user.role != Role.ADMIN)
                {
                    response.statusCode = 403; 
                    response.message = "No tienes permisos para acceder a este recurso.";
                    return new JsonResult(response);
                }

                
                response = _ordersService.GetAllOrders();
                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                response.statusCode = 500; 
                response.message = ex.Message;
                return new JsonResult(response);
            }
        }


        [Authorize]
        [HttpGet("{email}")]
        public ActionResult<AnyType> GetAll(string email)
        {
            Response response = new Response();

            try
            {
                string adminEmail = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                response = _ordersService.GetAll(adminEmail, email);

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
        [HttpGet("own")]
        public ActionResult<AnyType> GetOwn()
        {
            Response response = new Response();

            try
            {
                string email = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                response = _ordersService.GetOwn(email);

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
        [HttpPost("place")]
        public ActionResult<AnyType> Place([FromBody] PlaceOrderDTO model)
        {
            Response response = new Response();
            try
            {
                Console.WriteLine($"Received model: {JsonConvert.SerializeObject(model)}");

                string email = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                response = _ordersService.Place(email, model.Orders);

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
        [HttpPost("remove/{orderId}")]
        public ActionResult<AnyType> Remove(int orderId)
        {
            Response response = new Response();
            try
            {
                string email = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                response = _ordersService.Remove(email, orderId);

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
        [HttpDelete("{orderId}")]
        public ActionResult<Response> DeleteOrder(int orderId)
        {
            Response response = new Response();
            try
            {
                string email = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;
                response = _ordersService.Remove(email, orderId);

                if (response.statusCode != 200)
                    return BadRequest(response.message);

                return Ok(response.message);
            }
            catch (Exception e)
            {
                response.statusCode = 500;
                response.message = e.Message;
                return new JsonResult(response);
            }
        }

        [Authorize]
        [HttpGet("products/{orderId}")]
        public ActionResult<AnyType> GetOrderProducts(int orderId)
        {
            Response response = new Response();

            try
            {
                response = _ordersService.GetOrderProducts(orderId);
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
