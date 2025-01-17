using ViandasDelSur.Models;

namespace ViandasDelSur.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Product GetById(int id);
        void Save(Product product);
        void Remove(Product product);
        void SaveProductWithImage(Product product, Image image);

        IEnumerable<Product> GetByIds(IEnumerable<int> ids);
    }
}
