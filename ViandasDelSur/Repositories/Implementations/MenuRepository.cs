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
            return FindByCondition(m => !m.IsDeleted)  // Excluir menús eliminados
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
            // Opcionalmente implementar un soft delete en lugar de eliminar completamente
            menu.IsDeleted = true;
            Save(menu);
            //Delete(menu);
            //SaveChanges();
        }



        public void SaveMenuWithProducts(Menu menu)
        {
            // Desacoplar los productos para evitar que Entity Framework los duplique
            foreach (var product in menu.Products)
            {
                // Si el producto es nuevo (ID == 0), lo agregamos
                if (product.Id == 0)
                {
                    RepositoryContext.Products.Add(product);
                }
                else
                {
                    // Si el producto ya existe, lo actualizamos
                    RepositoryContext.Entry(product).State = EntityState.Modified;
                }
            }

            // Guardamos los cambios del menú
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


        /*public void SaveMenuWithProducts(Menu menu)
        {
            // Desvincular el menú si ya está rastreado
            var trackedMenu = RepositoryContext.ChangeTracker.Entries<Menu>()
                                  .FirstOrDefault(e => e.Entity.Id == menu.Id)?.Entity;

            if (trackedMenu != null)
            {
                RepositoryContext.Entry(trackedMenu).State = EntityState.Detached;
            }

            // Desvincular los productos que ya están rastreados
            foreach (var product in menu.Products)
            {
                var trackedProduct = RepositoryContext.ChangeTracker.Entries<Product>()
                                      .FirstOrDefault(e => e.Entity.Id == product.Id)?.Entity;

                if (trackedProduct != null)
                {
                    RepositoryContext.Entry(trackedProduct).State = EntityState.Detached;
                }

                // Establecer el estado de los productos: si tienen Id 0 se agregan, si no, se actualizan
                RepositoryContext.Entry(product).State = product.Id == 0 ? EntityState.Added : EntityState.Modified;
            }

            // Establecer el estado del menú: si tiene Id 0 se agrega, si no, se actualiza
            RepositoryContext.Entry(menu).State = menu.Id == 0 ? EntityState.Added : EntityState.Modified;

            // Guardar los cambios
            SaveChanges();
        }*/


        /*public void SaveMenuWithProducts(Menu menu)
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
        }*/

    }
}
