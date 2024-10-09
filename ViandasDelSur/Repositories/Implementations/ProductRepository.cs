/*using Microsoft.EntityFrameworkCore;
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
                .Include(p => p.Menu)
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
}*/


using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using ViandasDelSur.Models;
using ViandasDelSur.Repositories.Interfaces;

namespace ViandasDelSur.Repositories.Implementations
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        // Agregamos la palabra clave 'new' para ocultar el miembro de la clase base
        public new VDSContext RepositoryContext { get; }

        public ProductRepository(VDSContext repositoryContext) : base(repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }

        public ICollection<Product> GetAll()
        {
            return FindAll().ToList();
        }

        public Product GetById(int id)
        {
            return FindByCondition(p => p.Id == id)
                .Include(p => p.Image)
                .Include(p => p.Menu)
                .FirstOrDefault();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await FindByCondition(p => p.Id == id)
                .Include(p => p.Image)
                .Include(p => p.Menu)
                .FirstOrDefaultAsync();
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

        public async Task SaveAsync(Product product)
        {
            if (product.Id == 0)
            {
                Create(product);
            }
            else
            {
                Update(product);
            }

            await RepositoryContext.SaveChangesAsync();
        }

        public void Remove(Product product)
        {
            Delete(product);
            SaveChanges();
        }

        public void SaveProductWithImage(Product product, Image image)
        {
            // Revisa si la imagen ya está siendo rastreada
            var trackedEntity = RepositoryContext.ChangeTracker.Entries<Image>()
                                  .FirstOrDefault(e => e.Entity.Id == image.Id)?.Entity;

            if (trackedEntity != null)
            {
                product.Image = trackedEntity;
            }
            else
            {
                RepositoryContext.Attach(image);
                product.Image = image;
            }

            Save(product);
        }

        public new void SaveChanges()
        {
            RepositoryContext.SaveChanges();
        }

    }
}
