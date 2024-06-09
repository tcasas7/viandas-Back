using ViandasDelSur.Models;

namespace ViandasDelSur.Repositories.Interfaces
{
    public interface IMenuRepository
    {
        Menu GetById(int id);
        ICollection<Menu> GetAll();
        void Save(Menu menu);
        void Remove(Menu menu);
    }
}
