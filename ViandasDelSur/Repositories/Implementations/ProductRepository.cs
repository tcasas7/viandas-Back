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

        public void Remove(Product product)
        {
            Delete(product);
            SaveChanges();
        }

        public void SaveProductWithImage(Product product, Image image)
        {
            // Verificar si la imagen ya está siendo rastreada
            var trackedEntity = RepositoryContext.ChangeTracker.Entries<Image>()
                                  .FirstOrDefault(e => e.Entity.Id == image.Id)?.Entity;

            if (trackedEntity != null)
            {
                // Usar la entidad ya rastreada en lugar de adjuntar una nueva instancia
                product.Image = trackedEntity;
            }
            else
            {
                // Adjuntar la entidad si no está siendo rastreada
                RepositoryContext.Attach(image);
                product.Image = image;
            }

            Save(product);
        }
    }
}
