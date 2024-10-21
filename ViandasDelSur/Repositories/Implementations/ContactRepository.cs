using ViandasDelSur.Models;
using ViandasDelSur.Repositories.Interfaces;

namespace ViandasDelSur.Repositories.Implementations
{
    public class ContactRepository : RepositoryBase<Contact>, IContactRepository
    {
        public ContactRepository(VDSContext repositoryContext) : base(repositoryContext)
        {
        }

        public ICollection<Contact> GetAll()
        {
            return FindAll().ToList();
        }
        public Contact GetById(int id)
        {
            return FindByCondition(c => c.Id == id)
                .FirstOrDefault();
        }
        public Contact GetActive()
        {
            return FindByCondition(c => c.IsActive)
                .FirstOrDefault();
        }
        public void Save(Contact contact)
        {
            if (contact.Id == 0)
            {
                Create(contact);
            }
            else
            {
                Update(contact);
            }
            SaveChanges();
        }
        public void Remove(Contact contact)
        {
            Delete(contact);
            SaveChanges();
        }
    }
}
