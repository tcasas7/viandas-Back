using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
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
        private readonly VDSContext _dbContext;
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IVerificationService _verificationService;
        private readonly IDeliveryRepository _deliveryRepository;
        private readonly IProductRepository _productRepository;
        private readonly ISaleDataRepository _saleDataRepository;

        public OrdersService(
            VDSContext dbContext,
            IUserRepository userRepository,
            IOrderRepository orderRepository,
            IVerificationService verificationService,
            IDeliveryRepository deliveryRepository,
            IProductRepository productRepository,
            ISaleDataRepository saleDataRepository)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _verificationService = verificationService;
            _deliveryRepository = deliveryRepository;
            _productRepository = productRepository;
            _saleDataRepository = saleDataRepository;
        }

        public Response GetDates(string adminEmail)
        {
            Response response = new Response();

            var adminUser = _userRepository.FindByEmail(adminEmail);

            if (adminUser == null)
            {
                response.statusCode = 401;
                response.message = "Sesión invalida";
                return response;
            }

            response = _verificationService.VerifyAdmin(adminUser);

            if (response.statusCode != 200)
                return response;

            List<DateTime> dates = new List<DateTime>();

            var orders = _orderRepository.GetOrders();

            foreach (Order order in orders)
            {
                foreach (Delivery delivery in order.Deliveries)
                {
                    if (!dates.Contains(delivery.deliveryDate))
                    {
                        dates.Add(delivery.deliveryDate);
                    }
                }
            }

            response = new ResponseCollection<DateTime>(200, "Ok", dates);

            return response;
        }

        public Response GetAll(string adminEmail, string email)
        {
            Response response = new Response();

            var adminUser = _userRepository.FindByEmail(adminEmail);

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

            foreach (var order in user.Orders)
            {
                List<Delivery> del = _deliveryRepository.GetByOrder(order.Id).ToList();
                order.Deliveries = del;
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

            if (model == null)
            {
                response.statusCode = 400;
                response.message = "Error";
                return response;
            }

            foreach (var modelOrder in model)
            {
                if (modelOrder.price != 0)
                {
                    Order order = new Order
                    {
                        Id = modelOrder.Id,
                        price = modelOrder.price,
                        paymentMethod = modelOrder.paymentMethod,
                        hasSalt = modelOrder.hasSalt,
                        orderDate = modelOrder.orderDate,
                        userId = user.Id,
                        location = modelOrder.location,
                        description = modelOrder.description,
                        Deliveries = new List<Delivery>()
                    };

                    foreach (var deliveryDTO in modelOrder.deliveries)
                    {
                        if (deliveryDTO.quantity != 0)
                        {
                            var product = _productRepository.GetById(deliveryDTO.productId);

                            if (product != null)
                            {
                                Delivery delivery = new Delivery
                                {
                                    orderId = order.Id,
                                    productId = deliveryDTO.productId,
                                    delivered = false,
                                    deliveryDate = DatesTool.GetNextWeekDay(deliveryDTO.deliveryDate),  // No necesitamos cambiar el tipo de dato aquí
                                    quantity = deliveryDTO.quantity
                                };

                                order.Deliveries.Add(delivery);

                                SaleData saleData = new SaleData
                                {
                                    price = product.Menu.price,
                                    quantity = delivery.quantity,
                                    paymentMethod = modelOrder.paymentMethod,
                                    day = deliveryDTO.deliveryDate,  // Usamos directamente el número del día (1-5)
                                    productName = product.name,
                                    category = product.Menu.category,
                                    validDate = product.Menu.validDate
                                };

                                _saleDataRepository.Save(saleData);
                            }
                            else
                            {
                                response.statusCode = 400;
                                response.message = "Error al realizar la orden: Producto no encontrado";
                                return response;
                            }
                        }
                    }

                    _orderRepository.Save(order);
                }
            }

            response.statusCode = 200;
            response.message = "Ok";
            return response;
        }


        public Response Remove(string email, int orderId)
        {
            Response response = new Response();

            try
            {
                // Obtener el usuario actual por email
                var user = _userRepository.FindByEmail(email);

                if (user == null)
                {
                    response.statusCode = 401;
                    response.message = "Sesión inválida.";
                    return response;
                }

                // Buscar la orden por ID
                var order = _orderRepository.GetById(orderId);

                if (order == null)
                {
                    response.statusCode = 404;
                    response.message = "Orden no encontrada.";
                    return response;
                }

                // Verificar si la orden pertenece al usuario actual
                if (order.userId != user.Id)
                {
                    response.statusCode = 403;
                    response.message = "No tienes permisos para cancelar esta orden.";
                    return response;
                }

                // Eliminar la orden
                _orderRepository.Remove(order);

                // Guardar los cambios en la base de datos
                _dbContext.SaveChanges();

                response.statusCode = 200;
                response.message = "Orden cancelada con éxito.";
            }
            catch (Exception ex)
            {
                response.statusCode = 500;
                response.message = $"Error al cancelar la orden: {ex.Message}";
            }

            return response;
        }

        public List<Product> GetProductsByOrderId(int orderId)
        {
            return _orderRepository.GetProductsByOrderId(orderId);
        }



        public Response GetOrderProducts(int orderId)
        {
            Response response = new Response();

            try
            {
                var order = _orderRepository.GetById(orderId);
                if (order == null)
                {
                    response.statusCode = 404;
                    response.message = "Orden no encontrada";
                    return response;
                }

                var products = _orderRepository.GetProductsByOrderId(orderId); // Asegúrate de que el repositorio tenga este método implementado
                response.statusCode = 200;
                response.message = "Productos obtenidos exitosamente";
                response.data = products; // Usar 'response.data' para devolver los productos en el objeto de respuesta
            }
            catch (Exception ex)
            {
                response.statusCode = 500;
                response.message = ex.Message;
            }

            return response;
        }


    }
}
