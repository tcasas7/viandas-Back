using ViandasDelSur.Models;
using ViandasDelSur.Repositories.Interfaces;

namespace ViandasDelSur.Repositories.Implementations
{
    public class SaleDataRepository : RepositoryBase<SaleData>, ISaleDataRepository
    {
        public SaleDataRepository(VDSContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<SaleData> GetAll()
        {
            return FindAll().ToList();
        }

        public IEnumerable<SaleData> GetByValidDate(DateTime validDate)
        {
            return FindByCondition(sd => sd.validDate == validDate).ToList();
        }

        public void Save(SaleData saleData)
        {
            if (saleData.Id == 0)
            {
                Create(saleData);
            }
            else
            {
                Update(saleData);
            }

            SaveChanges();
        }
    }
}
