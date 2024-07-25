using Microsoft.EntityFrameworkCore;
using ViandasDelSur.Models;
using ViandasDelSur.Repositories.Interfaces;

namespace ViandasDelSur.Repositories.Implementations
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(VDSContext repositoryContext) : base(repositoryContext)
        {
        }
        public IEnumerable<object> GetOrders()
        {
            return FindAll()
                .Include(o => o.Deliveries)
                .ToList();
        }

        public Order GetById(int id)
        {
            return FindByCondition(o => o.Id == id)
                .Include(o => o.Deliveries)
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
