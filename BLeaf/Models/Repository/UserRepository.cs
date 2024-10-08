using BLeaf.Data;
using BLeaf.Models.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLeaf.Models.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public UserRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IEnumerable<User> AllUsers => _applicationDbContext.Users.OrderBy(u => u.FullName);

        public async Task<User> SaveUser(User user)
        {
            _applicationDbContext.Users.Add(user);
            await _applicationDbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUser(User user)
        {
            _applicationDbContext.Users.Update(user);
            await _applicationDbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> DeleteUser(int userId)
        {
            var user = await _applicationDbContext.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }

            _applicationDbContext.Users.Remove(user);
            await _applicationDbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> FindUserById(int userId)
        {
            return await _applicationDbContext.Users.FindAsync(userId);
        }

        public async Task<User> FindUserByEmail(string email)
        {
            return await _applicationDbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
        }
    }
}