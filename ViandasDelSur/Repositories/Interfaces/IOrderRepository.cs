using ViandasDelSur.Models;

namespace ViandasDelSur.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetOrders(int userId);
        void Save(Order order);
        Order GetById(int id);
        void Remove(Order order);
    }
}
