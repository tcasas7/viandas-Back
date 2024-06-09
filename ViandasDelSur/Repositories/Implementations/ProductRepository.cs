using Microsoft.EntityFrameworkCore;
using ViandasDelSur.Models;
using ViandasDelSur.Repositories.Interfaces;

namespace ViandasDelSur.Repositories.Implementations
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(VDSContext repositoryContext) : base(repositoryContext)
        {
        }

        public ICollection<Product> GetAll()
        {
            return FindAll().ToList();
        }

        public Product GetById(int id)
        {
            return FindByCondition(p => p.Id == id)
                .Include(p => p.Image)
                .FirstOrDefault();
        }

        public void Save(Product product)
        {
            if (product.Id == 0)
            {
                Create(product);
            }
            else
            {
                Update(product);
            }
            SaveChanges();
        }
    }
}
