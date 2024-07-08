using ViandasDelSur.Models;
using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models.Responses;
using ViandasDelSur.Repositories.Interfaces;
using ViandasDelSur.Services.Interfaces;
using ViandasDelSur.Tools;

namespace ViandasDelSur.Services.Implementations
{
    public class OrdersService : IOrdersService
    {
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IVerificationService _verificationService;

        public OrdersService(
            IUserRepository userRepository,
            IOrderRepository orderRepository,
            IVerificationService verificationService)
        {
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _verificationService = verificationService;
        }

        public Response GetAll(string adminEmail, string email)
        {
            Response response = new Response();

            var adminUser = _userRepository.FindByEmail(email);

            if (adminUser == null)
            {
                response.statusCode = 401;
                response.message = "Sesión invalida";
                return response;
            }

            response = _verificationService.VerifyAdmin(adminUser);

            if (response.statusCode != 200)
            {
                response.statusCode = 401;
                response.message = "Sesión invalida";
                return response;
            }

            var user = _userRepository.FindByEmail(email);

            if (user == null)
            {
                response.statusCode = 404;
                response.message = "Usuario no encontrado";
                return response;
            }

            if (user.Orders == null)
            {
                response.statusCode = 404;
                response.message = "Ocurrio un error";
                return response;
            }

            List<OrderDTO> result = new List<OrderDTO>();

            foreach (var order in user.Orders)
            {
                OrderDTO orderDTO = new OrderDTO(order);
                result.Add(orderDTO);
            }

            response = new ResponseCollection<OrderDTO>(200, "Ok", result);

            return response;
        }

        public Response GetOwn(string email)
        {
            Response response = new Response();

            var user = _userRepository.FindByEmail(email);

            if (user == null)
            {
                response.statusCode = 404;
                response.message = "Usuario no encontrado";
                return response;
            }

            if (user.Orders == null)
            {
                response.statusCode = 404;
                response.message = "Ocurrio un error";
                return response;
            }

            List<OrderDTO> result = new List<OrderDTO>();

            foreach (var order in user.Orders)
            {
                OrderDTO orderDTO = new OrderDTO(order);
                result.Add(orderDTO);
            }

            response = new ResponseCollection<OrderDTO>(200, "Ok", result);

            return response;
        }

        public Response Place(string email, ICollection<OrderDTO> model)
        {
            Response response = new Response();

            var user = _userRepository.FindByEmail(email);

            if (user == null)
            {
                response.statusCode = 404;
                response.message = "Usuario no encontrado";
                return response;
            }

            if(model == null)
            {
                response.statusCode = 400;
                response.message = "Error";
                return response;
            }

            foreach (var modelOrder in model)
            {
                Order order = new Order();

                order.Id = modelOrder.Id;
                order.price = modelOrder.price;
                order.paymentMethod = modelOrder.paymentMethod;
                order.hasSalt = modelOrder.hasSalt;
                order.orderDate = modelOrder.orderDate ;
                order.Deliveries = new List<Delivery>();
                order.userId = user.Id;
                order.location = modelOrder.location;

                foreach (var deliveryDTO in modelOrder.deliveries)
                {
                    Delivery delivery = new Delivery();
                    delivery.productId = deliveryDTO.productId;
                    delivery.delivered = false;
                    delivery.deliveryDate = DatesTool.GetNextDay(deliveryDTO.deliveryDate);
                    delivery.quantity = deliveryDTO.quantity;
                    order.Deliveries.Add(delivery);
                }

                _orderRepository.Save(order);
            }

            response.statusCode = 200;
            response.message = "Ok";
            return response;
        }

        public Response Remove(string email, int orderId)
        {
            Response response = new Response();

            var user = _userRepository.FindByEmail(email);

            if (user == null)
            {
                response.statusCode = 401;
                response.message = "Sesión invalida";
                return response;
            }

            var order = _orderRepository.GetById(orderId);

            if (order == null)
            {
                response.statusCode = 404;
                response.message = "Orden no encontrada";
                return response;
            }

            if (order.userId != user.Id)
            {
                response.statusCode = 403;
                response.message = "Sesión invalida";
                return response;
            }

            _orderRepository.Remove(order);

            response.statusCode = 200;
            response.message = "Ok";
            return response;
        }
    }
}
