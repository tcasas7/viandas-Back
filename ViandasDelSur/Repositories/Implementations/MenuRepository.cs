using Microsoft.EntityFrameworkCore;
using ViandasDelSur.Models;
using ViandasDelSur.Repositories.Interfaces;

namespace ViandasDelSur.Repositories.Implementations
{
    public class MenuRepository : RepositoryBase<Menu>, IMenuRepository
    {
        public MenuRepository(VDSContext repositoryContext) : base(repositoryContext)
        {
        }

        public ICollection<Menu> GetAll()
        {
            return FindAll()
                .Include(m => m.Products)
                .ThenInclude(p => p.Image)
                .ToList();
        }

        public Menu GetById(int id)
        {
            return FindByCondition(m => m.Id == id)
                .Include(m => m.Products)
                .ThenInclude(p => p.Image)
                .FirstOrDefault();
        }

        public void Save(Menu menu)
        {
            if (menu.Id == 0)
            {
                Create(menu);
            }
            else
            {
                Update(menu);
            }
            SaveChanges();
        }

        public void Remove(Menu menu)
        {
            Delete(menu);
            SaveChanges();
        }

        public void SaveMenuWithProducts(Menu menu)
        {
            // Primero, asegurarse de que el menú esté en el contexto
            var trackedMenu = RepositoryContext.ChangeTracker.Entries<Menu>()
                                 .FirstOrDefault(e => e.Entity.Id == menu.Id)?.Entity;

            if (trackedMenu != null)
            {
                RepositoryContext.Entry(trackedMenu).State = EntityState.Detached;
            }

            // Asegúrate de que los productos están siendo manejados correctamente
            foreach (var product in menu.Products)
            {
                var trackedProduct = RepositoryContext.ChangeTracker.Entries<Product>()
                                      .FirstOrDefault(e => e.Entity.Id == product.Id)?.Entity;

                if (trackedProduct != null)
                {
                    RepositoryContext.Entry(trackedProduct).State = EntityState.Detached;
                }

                // Configura el estado de Product para ser agregado o modificado
                RepositoryContext.Entry(product).State = product.Id == 0 ? EntityState.Added : EntityState.Modified;
            }

            // Configura el estado de Menu para ser agregado o modificado
            RepositoryContext.Entry(menu).State = menu.Id == 0 ? EntityState.Added : EntityState.Modified;

            // Guarda los cambios
            SaveChanges();
        }

    }
}
