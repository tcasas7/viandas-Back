using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Newtonsoft.Json;
using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models.Responses;
using ViandasDelSur.Services.Interfaces;

namespace ViandasDelSur.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _ordersService;

        public OrdersController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
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
        [HttpGet("{email}")]
        public ActionResult<AnyType> GetAll(string email)
        {
            Response response = new Response();

            try
            {
                string adminEmail = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                response = _ordersService.GetAll(adminEmail,email);

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
        public ActionResult<AnyType> Place( [FromBody] PlaceOrderDTO model)
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
    }
}
