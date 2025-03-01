using Microsoft.EntityFrameworkCore;
using ViandasDelSur.Models;
using ViandasDelSur.Repositories.Interfaces;

namespace ViandasDelSur.Repositories.Implementations
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        private readonly VDSContext _context;

        
        public OrderRepository(VDSContext repositoryContext) : base(repositoryContext)
        {
            _context = repositoryContext;
        }

        public List<Product> GetProductsByOrderId(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.Deliveries)
                .ThenInclude(d => d.Product)
                .FirstOrDefault(o => o.Id == orderId);

            return order?.Deliveries.Select(d =>
            {
                d.MenuId = d.MenuId; // Asegúrate de cargar la propiedad
                return d.Product;
            }).ToList() ?? new List<Product>();
        }


        public IEnumerable<Order> GetOrders()
        {
            return FindAll()
                .Include(o => o.Deliveries)
                .ThenInclude(d => d.Product) 
                .ThenInclude(p => p.Menu)
                .Include(o => o.User)
                .ToList();
        }

        public Order GetById(int id)
        {
            return FindByCondition(o => o.Id == id)
                .Include(o => o.Deliveries)
                .ThenInclude(d => d.Product)
                .FirstOrDefault();
        }


        public IEnumerable<Order> GetOrders(int userId)
        {
            return FindByCondition(o => o.userId == userId)
                .Include(o => o.Deliveries)
                .ToList();
        }
        public void Save(Order order)
        {
            foreach (var delivery in order.Deliveries)
            {
                
                delivery.MenuId = delivery.MenuId;
                delivery.orderId = order.Id;
            }

            if (order.Id == 0)
            {
                Create(order);
            }
            else
            {
                Update(order);
            }

            SaveChanges();
        }

        public void Remove(Order order)
        {
            Delete(order);
            SaveChanges();
        }
    }
}
