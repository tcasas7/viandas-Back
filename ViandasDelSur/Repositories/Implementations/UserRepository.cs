using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System;
using ViandasDelSur.Models;
using ViandasDelSur.Repositories.Interfaces;

namespace ViandasDelSur.Repositories.Implementations
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(VDSContext repositoryContext) : base(repositoryContext) { }

        public User FindByEmail(string email)
        {
            return FindByCondition(u => u.email == email)
                .Include(u => u.Locations)
                .Include(u => u.Orders)
            .FirstOrDefault();
        }

        public User FindById(long id)
        {
            return FindByCondition(u => u.Id == id)
                .Include(u => u.Locations)
                .Include(u => u.Orders)
                .FirstOrDefault();
        }

        public IEnumerable<User> GetAllUsers()
        {
            return FindAll()
                .Include(u => u.Locations)
                .Include(u => u.Orders)
                .ToList();
        }

        public void Save(User user)
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
    }
}
