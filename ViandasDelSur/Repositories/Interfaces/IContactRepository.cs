using ViandasDelSur.Models;

namespace ViandasDelSur.Repositories.Interfaces
{
    public interface IContactRepository
    {
        ICollection<Contact> GetAll();
        Contact GetById(int Id);
        Contact GetActive();
        void Save(Contact contact);
        void Remove(Contact contact);
    }
}
