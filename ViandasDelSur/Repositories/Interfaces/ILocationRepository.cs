using System.Collections.Generic;
using ViandasDelSur.Models;

namespace ViandasDelSur.Repositories.Interfaces
{
    public interface ILocationRepository
    {
        
        bool Save(Location location);
        Location FindById(long id);
        Location FindByUserIdAndDir(long userId, string dir);
        Location GetDefault(string email);
        IEnumerable<Location> GetLocationsByUserId(long userId);
        void Remove(Location location);
    }
}
