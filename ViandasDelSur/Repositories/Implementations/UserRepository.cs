using Microsoft.EntityFrameworkCore;
using ViandasDelSur.Models;
using ViandasDelSur.Repositories.Interfaces;

namespace ViandasDelSur.Repositories.Implementations
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        protected readonly VDSContext _context;

        public UserRepository(VDSContext repositoryContext) : base(repositoryContext)
        {
            _context = repositoryContext;
        }

        public User FindByEmail(string email)
        {
            try
            {
                return FindByCondition(u => u.email == email)
                    .Include(u => u.Locations)
                    .Include(u => u.Orders)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al buscar usuario por email: {ex.Message}", ex);
            }
        }

        public User FindById(long id)
        {
            try
            {
                return FindByCondition(u => u.Id == id)
                    .Include(u => u.Locations)
                    .Include(u => u.Orders)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al buscar usuario por ID: {ex.Message}", ex);
            }
        }

        public IEnumerable<User> GetAllUsers()
        {
            try
            {
                return FindAll()
                    .Include(u => u.Locations)
                    .Include(u => u.Orders)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener todos los usuarios: {ex.Message}", ex);
            }
        }

        public IEnumerable<User> GetUnverifiedUsers()
        {
            try
            {
                return FindByCondition(u => !u.IsVerified)
                    .Include(u => u.Locations)
                    .Include(u => u.Orders)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener usuarios no verificados: {ex.Message}", ex);
            }
        }

        public void Save(User user)
        {
            try
            {
                if (user.Id == 0)
                {
                    Create(user);
                }
                else
                {
                    Update(user);
                }

                SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Error al guardar el usuario: {ex.Message}", ex);
            }
        }

        public void Remove(User user)
        {
            try
            {
                Delete(user);
                SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar el usuario: {ex.Message}", ex);
            }
        }

    }
}
