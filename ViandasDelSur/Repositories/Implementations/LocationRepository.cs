using ViandasDelSur.Models;
using ViandasDelSur.Repositories.Interfaces;

namespace ViandasDelSur.Repositories.Implementations
{
    public class LocationRepository : RepositoryBase<Location>, ILocationRepository
    {
        private readonly VDSContext _context;
        public LocationRepository(VDSContext context) : base(context)
        {
            _context = context;
        }

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

        public Location FindByUserIdAndDir(long userId, string dir)
        {
            try
            {
                return _context.Locations
                    .FirstOrDefault(l => l.userId == userId && l.dir == dir);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al buscar ubicación por usuario y dirección: {ex.Message}", ex);
            }
        }

        // Guardar una nueva ubicación
        public bool Save(Location location)
        {
            try
            {
                _context.Locations.Add(location);
                _context.SaveChanges(); // Guarda los cambios en la base de datos.
                return true; // Retorna true si fue exitoso.
            }
            catch
            {
                return false; // Retorna false en caso de error.
            }
        }

        // Obtener todas las ubicaciones de un usuario
        public IEnumerable<Location> GetLocationsByUserId(long userId)
        {
            try
            {
                return _context.Locations
                    .Where(l => l.userId == userId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener ubicaciones del usuario: {ex.Message}", ex);
            }
        }
    }
}
