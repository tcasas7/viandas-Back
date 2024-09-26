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
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IVerificationService _verificationService;
        private readonly IDeliveryRepository _deliveryRepository;
        private readonly IProductRepository _productRepository;
        private readonly ISaleDataRepository _saleDataRepository;

        public OrdersService(
            IUserRepository userRepository,
            IOrderRepository orderRepository,
            IVerificationService verificationService,
            IDeliveryRepository deliveryRepository,
            IProductRepository productRepository,
            ISaleDataRepository saleDataRepository)
        {
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
                    Order order = new Order();

                    order.Id = modelOrder.Id;
                    order.price = modelOrder.price;
                    order.paymentMethod = modelOrder.paymentMethod;
                    order.hasSalt = modelOrder.hasSalt;
                    order.orderDate = modelOrder.orderDate;
                    order.Deliveries = new List<Delivery>();
                    order.userId = user.Id;
                    order.location = modelOrder.location;
                    order.description = modelOrder.description;

                    foreach (var deliveryDTO in modelOrder.deliveries)
                    {
                        if (deliveryDTO.quantity != 0)
                        {
                            var product = _productRepository.GetById(deliveryDTO.productId);

                            if (product != null)
                            {
                                Delivery delivery = new Delivery();
                                delivery.orderId = order.Id;
                                delivery.productId = deliveryDTO.productId;
                                delivery.delivered = false;

                                // Conversión de número (1-5) a DayOfWeek
                                DayOfWeek dayOfWeek = deliveryDTO.deliveryDate switch
                                {
                                    1 => DayOfWeek.Monday,
                                    2 => DayOfWeek.Tuesday,
                                    3 => DayOfWeek.Wednesday,
                                    4 => DayOfWeek.Thursday,
                                    5 => DayOfWeek.Friday,
                                    _ => throw new Exception("Día inválido")
                                };

                                // Asignar el día convertido
                                delivery.deliveryDate = DatesTool.GetNextWeekDay(dayOfWeek);

                                delivery.quantity = deliveryDTO.quantity;
                                order.Deliveries.Add(delivery);

                                // Guardar la venta
                                SaleData saleData = new SaleData();
                                saleData.price = product.Menu.price;
                                saleData.quantity = delivery.quantity;
                                saleData.paymentMethod = modelOrder.paymentMethod;
                                saleData.day = (DayOfWeek)(deliveryDTO.deliveryDate % 7); // Convertimos el número de día al tipo DayOfWeek
                                saleData.productName = product.name;
                                saleData.category = product.Menu.category;
                                saleData.validDate = product.Menu.validDate;

                                _saleDataRepository.Save(saleData);
                            }
                            else
                            {
                                response.statusCode = 400;
                                response.message = "Error al realizar la orden";
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





        /*public Response Place(string email, ICollection<OrderDTO> model)
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
                if(modelOrder.price != 0)
                {
                    Order order = new Order();

                    order.Id = modelOrder.Id;
                    order.price = modelOrder.price;
                    order.paymentMethod = modelOrder.paymentMethod;
                    order.hasSalt = modelOrder.hasSalt;
                    order.orderDate = modelOrder.orderDate;
                    order.Deliveries = new List<Delivery>();
                    order.userId = user.Id;
                    order.location = modelOrder.location;
                    order.description = modelOrder.description;

                    foreach (var deliveryDTO in modelOrder.deliveries)
                    {
                        if (deliveryDTO.quantity != 0)
                        {
                            var product = _productRepository.GetById(deliveryDTO.productId);

                            if (product != null)
                            {
                                Delivery delivery = new Delivery();
                                delivery.orderId = order.Id;
                                delivery.productId = deliveryDTO.productId;
                                delivery.delivered = false;
                                delivery.deliveryDate = DatesTool.GetNextWeekDay(deliveryDTO.deliveryDate);
                                delivery.quantity = deliveryDTO.quantity;
                                order.Deliveries.Add(delivery);

                                SaleData saleData = new SaleData();

                                saleData.price = product.Menu.price;
                                saleData.quantity = delivery.quantity;
                                saleData.paymentMethod = modelOrder.paymentMethod;
                                saleData.day = deliveryDTO.deliveryDate;
                                saleData.productName = product.name;
                                saleData.category = product.Menu.category;
                                saleData.validDate = product.Menu.validDate;

                                _saleDataRepository.Save(saleData);
                            }
                            else
                            {
                                response.statusCode = 400;
                                response.message = "Error al realizar la orden";
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
        }*/

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
