using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using ViandasDelSur.Models;
using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models.Responses;
using ViandasDelSur.Repositories.Interfaces;
using ViandasDelSur.Services.Interfaces;

namespace ViandasDelSur.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IVerificationService _verificationService;
        public OrdersController(
            IUserRepository userRepository,
            IOrderRepository orderRepository,
            IVerificationService verificationService
            )
        {
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _verificationService = verificationService;
        }

        [Authorize]
        [HttpGet]
        public ActionResult<AnyType> GetAll(string email)
        {
            Response response = new Response();

            try
            {
                string adminEmail = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                var adminUser = _userRepository.FindByEmail(email);

                if (adminUser == null)
                {
                    response.statusCode = 401;
                    response.message = "Sesión invalida";
                    return new JsonResult(response);
                }

                response = _verificationService.VerifyAdmin(adminUser);

                if (response.statusCode != 200)
                {
                    response.statusCode = 401;
                    response.message = "Sesión invalida";
                    return new JsonResult(response);
                }

                var user = _userRepository.FindByEmail(email);

                if (user == null)
                {
                    response.statusCode = 404;
                    response.message = "Usuario no encontrado";
                    return new JsonResult(response);
                }

                if (user.Orders == null)
                {
                    response.statusCode = 404;
                    response.message = "Ocurrio un error";
                    return new JsonResult(response);
                }

                List<OrderDTO> result = new List<OrderDTO>();

                foreach (var order in user.Orders)
                {
                    OrderDTO orderDTO = new OrderDTO(order);
                    result.Add(orderDTO);
                }

                response = new ResponseCollection<OrderDTO>(200, "Ok", result);

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

                var user = _userRepository.FindByEmail(email);

                if (user == null)
                {
                    response.statusCode = 404;
                    response.message = "Usuario no encontrado";
                    return new JsonResult(response);
                }

                if (user.Orders == null)
                {
                    response.statusCode = 404;
                    response.message = "Ocurrio un error";
                    return new JsonResult(response);
                }

                List<OrderDTO> result = new List<OrderDTO>();

                foreach (var order in user.Orders)
                {
                    OrderDTO orderDTO = new OrderDTO(order);
                    result.Add(orderDTO);
                }

                response = new ResponseCollection<OrderDTO>(200, "Ok", result);

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
        public ActionResult<AnyType> Place( [FromBody] OrderDTO model)
        {
            Response response = new Response();
            try
            {
                string email = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                var user = _userRepository.FindByEmail(email);

                if (user == null)
                {
                    response.statusCode = 404;
                    response.message = "Usuario no encontrado";
                    return new JsonResult(response);
                }

                Order order = new Order();

                order.Id = model.Id;
                order.price = model.price;
                order.paymentMethod = model.paymentMethod;
                order.hasSalt = model.hasSalt;
                order.orderDate = model.orderDate;
                order.Deliveries = new List<Delivery>();

                foreach (var deliveryDTO in model.deliveries)
                {
                    Delivery delivery = new Delivery();
                    delivery.productId = deliveryDTO.productId;
                    delivery.delivered = false;
                    delivery.deliveryDate = deliveryDTO.deliveryDate;
                    order.Deliveries.Add(delivery);
                }

                _orderRepository.Save(order);

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
        [HttpPost("remove/{orderId}")]
        public ActionResult<AnyType> Remove(int orderId)
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

                var order = _orderRepository.GetById(orderId);

                if (order == null)
                {
                    response.statusCode = 404;
                    response.message = "Orden no encontrada";
                    return new JsonResult(response);
                }

                if (order.userId != user.Id)
                {
                    response.statusCode = 403;
                    response.message = "Sesión invalida";
                    return new JsonResult(response);
                }

                _orderRepository.Remove(order);

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
