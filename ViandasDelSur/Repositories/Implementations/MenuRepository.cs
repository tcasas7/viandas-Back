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
    }
}
