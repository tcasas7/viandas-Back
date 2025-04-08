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
                Console.WriteLine($"📩 Received model: {JsonConvert.SerializeObject(model)}");

                // Obtener el email del usuario autenticado
                string email = User.FindFirst("Account")?.Value ?? string.Empty;

                // 🚨 Validación: Verificar si hay órdenes en el DTO
                if (model.Orders == null || !model.Orders.Any())
                {
                    response.statusCode = 400;
                    response.message = "No se enviaron órdenes en la solicitud.";
                    return new JsonResult(response);
                }

                // 📅 Obtener la hora actual (UTC convertida a hora Argentina)
                DateTime nowUtc = DateTime.UtcNow;

                foreach (var order in model.Orders)
                {
                    foreach (var delivery in order.deliveries)
                    {
                        DateTime deliveryDate = delivery.deliveryDate.Date;

                        Console.WriteLine($"📅 Validando pedido -> Fecha entrega: {deliveryDate}");
                       
                        if (!EsPedidoValido(deliveryDate, nowUtc))
                        {
                            return BadRequest(new { message = $"No se puede realizar el pedido para la fecha {deliveryDate:dd/MM/yyyy}." });
                        }
                    }
                }

                // ✅ Llamar al servicio solo si todo es válido
                response = _ordersService.Place(email, model);

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

        private bool EsPedidoValido(DateTime fechaPedido, DateTime now)
        {
            var argentinaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");
            var ahora = TimeZoneInfo.ConvertTimeFromUtc(now, argentinaTimeZone);
            var hoy = ahora.Date;
            var hora = ahora.TimeOfDay;
            var diaDeLaSemana = ahora.DayOfWeek;

            var fechaPedidoDia = fechaPedido.Date;

            // 🚫 1. Bloqueo total: viernes desde las 8:00 AM hasta sábado 00:00
            if (diaDeLaSemana == DayOfWeek.Friday && hora >= TimeSpan.FromHours(8))
                return false;

            if (diaDeLaSemana == DayOfWeek.Saturday && hora < TimeSpan.FromHours(0))
                return false;

            // 🚫 2. No permitir pedidos para días anteriores
            if (fechaPedidoDia < hoy)
                return false;

            // 🚫 3. No permitir pedidos para el mismo día después de las 8:00 AM
            if (fechaPedidoDia == hoy && hora >= TimeSpan.FromHours(8))
                return false;

            // 🚫 4. No permitir pedidos para la semana siguiente (excepto fin de semana)
            int diasHastaLunesProximo = ((int)DayOfWeek.Monday + 7 - (int)diaDeLaSemana) % 7;
            DateTime proximoLunes = hoy.AddDays(diasHastaLunesProximo);
            if (fechaPedidoDia >= proximoLunes && diaDeLaSemana != DayOfWeek.Saturday && diaDeLaSemana != DayOfWeek.Sunday)
                return false;

            // ✅ Pedido válido
            return true;
        }


    }


}
