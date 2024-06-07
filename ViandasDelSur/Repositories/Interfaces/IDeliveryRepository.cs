using ViandasDelSur.Models;

namespace ViandasDelSur.Repositories.Interfaces
{
    public interface IDeliveryRepository
    {
        ICollection<Delivery> GetByOrder(int orderId);
        void Save(Delivery delivery);
    }
}
