using ViandasDelSur.Models;

namespace ViandasDelSur.Repositories.Interfaces
{
    public interface ILocationRepository
    {
        void Save(Location location);
        Location FindById(long id);
        void Remove(Location location);
    }
}
