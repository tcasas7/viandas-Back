using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Linq;
using ViandasDelSur.Models;
using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models.Enums;
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
                response.message = "Sesión inválida";
                return response;
            }

           
            if (adminUser.role == Role.ADMIN) 
            {
                
                var allOrders = _orderRepository.GetOrders().Cast<Order>().ToList();

              
                var result = allOrders.Select(order => new OrderDTO(order)).ToList();

                response = new ResponseCollection<OrderDTO>(200, "Órdenes obtenidas correctamente.", result);
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
                response.message = "El usuario no tiene órdenes.";
                return response;
            }

            
            var userOrders = user.Orders.Select(order => new OrderDTO(order)).ToList();

            response = new ResponseCollection<OrderDTO>(200, "Órdenes obtenidas correctamente.", userOrders);
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
                    deliveryDate = d.deliveryDate,
                    quantity = d.quantity,
                    MenuId = d.MenuId
                }).ToList()
            }).ToList();

            response = new ResponseCollection<OrderDTO>(200, "Ok", result);

            return response;
        }

        public Response GetAllOrders()
        {
            Response response = new Response();

            try
            {
                
                var allOrders = _orderRepository.GetOrders(); 

               
                var result = allOrders.Select(order => new OrderDTO(order)).ToList();

                response = new ResponseCollection<OrderDTO>(200, "Órdenes obtenidas correctamente.", result);
            }
            catch (Exception ex)
            {
                response.statusCode = 500;
                response.message = $"Error al obtener órdenes: {ex.Message}";
            }

            return response;
        }


        public Response Place(string email, ICollection<OrderDTO> model)
        {
            Response response = new Response();
            Console.WriteLine($"📩 Iniciando el método Place para el usuario: {email}");

            // 🔍 Verificar si el usuario existe
            var user = _userRepository.FindByEmail(email);
            if (user == null)
            {
                Console.WriteLine($"❌ Error: Usuario con email {email} no encontrado.");
                response.statusCode = 404;
                response.message = "Usuario no encontrado";
                return response;
            }

            // 🔍 Verificar si el modelo es válido
            if (model == null || model.Count == 0)
            {
                Console.WriteLine($"❌ Error: Modelo de orden vacío o inválido.");
                response.statusCode = 400;
                response.message = "Error en el modelo proporcionado";
                return response;
            }

            // 🛒 Crear la orden
            var modelOrder = model.First();
            Order order = new Order
            {
                paymentMethod = modelOrder.paymentMethod,
                hasSalt = modelOrder.hasSalt,
                orderDate = DateTime.UtcNow,
                userId = user.Id,
                location = modelOrder.location,
                description = modelOrder.description,
                Deliveries = new List<Delivery>()
            };

            Console.WriteLine($"📝 Creando orden para usuario ID: {user.Id} con método de pago {order.paymentMethod}");

            decimal totalPrice = 0;
            int totalPlates = 0;
            var menuQuantities = new Dictionary<int, int>();

            foreach (var modelOrderItem in model)
            {
                foreach (var deliveryDTO in modelOrderItem.deliveries)
                {
                    if (deliveryDTO.quantity <= 0) continue;

                    var product = _productRepository.GetById(deliveryDTO.productId);
                    if (product == null)
                    {
                        Console.WriteLine($"❌ Error: Producto con ID {deliveryDTO.productId} no encontrado.");
                        response.statusCode = 400;
                        response.message = $"Error al realizar la orden: Producto con ID {deliveryDTO.productId} no encontrado";
                        return response;
                    }

                    var menu = product.Menu;
                    if (menu == null)
                    {
                        Console.WriteLine($"❌ Error: Menú no encontrado para el producto {product.name}.");
                        response.statusCode = 400;
                        response.message = $"Error al realizar la orden: Menú no encontrado para el producto {product.name}";
                        return response;
                    }

                    // 🗓️ Calcular la fecha de entrega correcta
                    DateTime deliveryDate = DatesTool.GetNextWeekDay(deliveryDTO.deliveryDate);
                    Console.WriteLine($"📅 Fecha de entrega calculada: {deliveryDate} para el producto {product.name}");

                    // 📦 Crear la entrega
                    Delivery delivery = new Delivery
                    {
                        productId = product.Id,
                        delivered = false,
                        deliveryDate = deliveryDate,
                        quantity = deliveryDTO.quantity,
                        MenuId = product.MenuId
                    };

                    order.Deliveries.Add(delivery);
                    totalPlates += delivery.quantity;

                    if (menuQuantities.ContainsKey(menu.Id))
                        menuQuantities[menu.Id] += delivery.quantity;
                    else
                        menuQuantities[menu.Id] = delivery.quantity;

                    // Registrar la venta
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

                    Console.WriteLine($"💾 Guardando SaleData para producto {saleData.productName} con cantidad {saleData.quantity}");
                    _saleDataRepository.Save(saleData);
                }
            }

            // 🏷️ Calcular el precio total
            foreach (var entry in menuQuantities)
            {
                int MenuId = entry.Key;
                int menuPlates = entry.Value;

                var menu = _menuRepository.GetById(MenuId);
                if (menu == null)
                {
                    Console.WriteLine($"❌ Error: Menú con ID {MenuId} no encontrado.");
                    response.statusCode = 400;
                    response.message = $"Error al realizar la orden: Menú con ID {MenuId} no encontrado";
                    return response;
                }

                // Aplicar descuento
                if (menu.precioPromo.HasValue && totalPlates >= 4)
                    totalPrice += menu.precioPromo.Value * menuPlates;
                else
                    totalPrice += menu.price * menuPlates;
            }

            order.price = totalPrice;

            try
            {
                Console.WriteLine($"💾 Guardando orden...");
                _orderRepository.Save(order);
                Console.WriteLine($"✅ Orden guardada correctamente con ID: {order.Id}");

                // Verificar si las entregas tienen el `orderId`
                foreach (var delivery in order.Deliveries)
                {
                    Console.WriteLine($"📦 Entrega -> Producto ID: {delivery.productId}, Fecha: {delivery.deliveryDate}, Orden ID: {delivery.orderId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR al guardar la orden: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"🔍 Inner Exception: {ex.InnerException.Message}");
                }
                return new Response
                {
                    statusCode = 500,
                    message = $"Error al guardar la orden: {ex.InnerException?.Message ?? ex.Message}"
                };
            }

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
