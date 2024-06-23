using ViandasDelSur.Models;
using ViandasDelSur.Repositories.Interfaces;

namespace ViandasDelSur.Repositories.Implementations
{
    public class LocationRepository : RepositoryBase<Location>, ILocationRepository
    {
        public LocationRepository(VDSContext repositoryContext) : base(repositoryContext) { }

        public Location FindById(long id)
        {
            return FindByCondition(l => l.Id == id).FirstOrDefault();
        }

        public Location GetDefault(string email)
        {
            return FindByCondition(l => l.User.email == email && l.isDefault).FirstOrDefault();
        }

        public void Remove(Location location)
        {
            Delete(location);
            SaveChanges();
        }

        public void Save(Location location)
        {
            if (location.Id == 0)
            {
                Create(location);
            }
            else
            {
                Update(location);
            }

            SaveChanges();
        }
    }
}
