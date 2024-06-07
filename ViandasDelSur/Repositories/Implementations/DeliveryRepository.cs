using Microsoft.EntityFrameworkCore;
using ViandasDelSur.Models;
using ViandasDelSur.Repositories.Interfaces;

namespace ViandasDelSur.Repositories.Implementations
{
    public class DeliveryRepository : RepositoryBase<Delivery>, IDeliveryRepository
    {
        public DeliveryRepository(VDSContext repositoryContext) : base(repositoryContext)
        {
        }

        public ICollection<Delivery> GetByOrder(int orderId)
        {
            return FindByCondition(d => d.orderId == orderId)
                .Include(d => d.Product)
                .ToList();
        }

        public void Save(Delivery delivery)
        {
            if (delivery.Id == 0)
            {
                Create(delivery);
            }
            else
            {
                Update(delivery);
            }
            SaveChanges();
        }
    }
}
