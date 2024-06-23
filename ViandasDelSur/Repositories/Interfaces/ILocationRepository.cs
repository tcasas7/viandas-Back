using ViandasDelSur.Models;

namespace ViandasDelSur.Repositories.Interfaces
{
    public interface ILocationRepository
    {
        void Save(Location location);
        Location FindById(long id);
        Location GetDefault(string email);
        void Remove(Location location);
    }
}
