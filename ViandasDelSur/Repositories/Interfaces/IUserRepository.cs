using ViandasDelSur.Models;

namespace ViandasDelSur.Repositories.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAllUsers();
        IEnumerable<User> GetUnverifiedUsers();
        void Save(User user);
        void Remove(User user);
        void SaveChanges();
        User FindById(long id);
        User FindByEmail(string email);
    }
}
