using ViandasDelSur.Models;

namespace ViandasDelSur.Repositories.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAllUsers();
        void Save(User user);
        User FindById(long id);
        User FindByEmail(string email);
    }
}
