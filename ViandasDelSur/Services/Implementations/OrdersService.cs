using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using ViandasDelSur.Models;
using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models.Responses;
using ViandasDelSur.Repositories.Implementations;
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
        private readonly IMenuRepository _menuRepository;

        public OrdersService(
            VDSContext dbContext,
            IUserRepository userRepository,
            IOrderRepository orderRepository,
            IDeliveryRepository deliveryRepository,
            IProductRepository productRepository,
            ISaleDataRepository saleDataRepository,
            IVerificationService verificationService,
            IMenuRepository menuRepository)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _verificationService = verificationService;
            _deliveryRepository = deliveryRepository;
            _productRepository = productRepository;
            _saleDataRepository = saleDataRepository;
            _menuRepository = menuRepository;
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
                response.message = "Ocurrió un error";
                return response;
            }

            
            foreach (var order in user.Orders)
            {
                var deliveries = _deliveryRepository.GetByOrder(order.Id).ToList();
                order.Deliveries = deliveries;
            }

           
            var result = user.Orders.Select(order => new OrderDTO
            {
                Id = order.Id,
                price = order.price,
                paymentMethod = order.paymentMethod,
                hasSalt = order.hasSalt,
                orderDate = order.orderDate,
                location = order.location,
                description = order.description,
                deliveries = order.Deliveries.Select(d => new DeliveryDTO
                {
                    Id = d.Id,
                    productId = d.productId,
                    delivered = d.delivered,
                    deliveryDate = d.deliveryDate.DayOfWeek,
                    quantity = d.quantity,
                    MenuId = d.MenuId
                }).ToList()
            }).ToList();

            response = new ResponseCollection<OrderDTO>(200, "Ok", result);

            return response;
        }

        public Response Place(string email, ICollection<OrderDTO> model)
        {
            Response response = new Response();

            // Verificar el usuario
            var user = _userRepository.FindByEmail(email);
            if (user == null)
            {
                response.statusCode = 404;
                response.message = "Usuario no encontrado";
                return response;
            }

            // Verificar que el modelo es válido
            if (model == null || model.Count == 0)
            {
                response.statusCode = 400;
                response.message = "Error en el modelo proporcionado";
                return response;
            }

            // Crear una nueva orden
            var modelOrder = model.First();
            Order order = new Order
            {
                Id = modelOrder.Id,
                paymentMethod = modelOrder.paymentMethod,
                hasSalt = modelOrder.hasSalt,
                orderDate = DateTime.UtcNow,
                userId = user.Id,
                location = modelOrder.location,
                description = modelOrder.description,
                Deliveries = new List<Delivery>()
            };

            decimal totalPrice = 0;
            int totalPlates = 0;

            // Diccionario para agrupar platos por menú
            var menuQuantities = new Dictionary<int, int>(); // Clave: MenuId, Valor: Cantidad de platos

            foreach (var modelOrderItem in model)
            {
                foreach (var deliveryDTO in modelOrderItem.deliveries)
                {
                    if (deliveryDTO.quantity <= 0) continue;

                    var product = _productRepository.GetById(deliveryDTO.productId);

                    if (product == null)
                    {
                        response.statusCode = 400;
                        response.message = $"Error al realizar la orden: Producto con ID {deliveryDTO.productId} no encontrado";
                        return response;
                    }

                    var menu = product.Menu;
                    if (menu == null)
                    {
                        response.statusCode = 400;
                        response.message = $"Error al realizar la orden: Menú no encontrado para el producto {product.name}";
                        return response;
                    }

                    // Crear la entrega
                    Delivery delivery = new Delivery
                    {
                        productId = product.Id,
                        delivered = false,
                        deliveryDate = DatesTool.GetNextWeekDay(deliveryDTO.deliveryDate),
                        quantity = deliveryDTO.quantity,
                        MenuId = product.menuId
                    };

                    order.Deliveries.Add(delivery);

                    // Registrar la cantidad total de platos
                    totalPlates += delivery.quantity;

                    // Acumular platos por menú
                    if (menuQuantities.ContainsKey(menu.Id))
                    {
                        menuQuantities[menu.Id] += delivery.quantity;
                    }
                    else
                    {
                        menuQuantities[menu.Id] = delivery.quantity;
                    }

                    // Registrar en SaleData
                    SaleData saleData = new SaleData
                    {
                        price = menu.price,
                        quantity = deliveryDTO.quantity,
                        paymentMethod = modelOrder.paymentMethod,
                        day = deliveryDTO.deliveryDate,
                        productName = product.name,
                        category = menu.category,
                        validDate = menu.validDate
                    };

                    _saleDataRepository.Save(saleData);
                }
            }

            // Calcular el precio total
            foreach (var entry in menuQuantities)
            {
                int menuId = entry.Key;
                int menuPlates = entry.Value;

                var menu = _menuRepository.GetById(menuId);
                if (menu == null)
                {
                    response.statusCode = 400;
                    response.message = $"Error al realizar la orden: Menú con ID {menuId} no encontrado";
                    return response;
                }

                // Aplicar precio promocional si el total de platos es 4 o más
                if (menu.precioPromo.HasValue && totalPlates >= 4)
                {
                    totalPrice += menu.precioPromo.Value * menuPlates;
                }
                else
                {
                    totalPrice += menu.price * menuPlates;
                }
            }

            // Asignar el precio total calculado a la orden
            order.price = totalPrice;

            // Guardar la orden en la base de datos
            _orderRepository.Save(order);

            response.statusCode = 200;
            response.message = "Orden realizada con éxito";
            return response;
        }

        public Response Remove(string email, int orderId)
        {
            Response response = new Response();

            try
            {
                
                var user = _userRepository.FindByEmail(email);

                if (user == null)
                {
                    response.statusCode = 401;
                    response.message = "Sesión inválida.";
                    return response;
                }

               
                var order = _orderRepository.GetById(orderId);

                if (order == null)
                {
                    response.statusCode = 404;
                    response.message = "Orden no encontrada.";
                    return response;
                }

               
                if (order.userId != user.Id)
                {
                    response.statusCode = 403;
                    response.message = "No tienes permisos para cancelar esta orden.";
                    return response;
                }

                
                _orderRepository.Remove(order);

                
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

                var products = _orderRepository.GetProductsByOrderId(orderId); 
                response.statusCode = 200;
                response.message = "Productos obtenidos exitosamente";
                response.data = products; 
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
