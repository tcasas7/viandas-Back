using ViandasDelSur.Models;

namespace ViandasDelSur.Repositories.Interfaces
{
    public interface ISaleDataRepository
    {
        IEnumerable<SaleData> GetAll();
        IEnumerable<SaleData> GetByValidDate(DateTime validDate);
        void Save(SaleData saleData);
    }
}
