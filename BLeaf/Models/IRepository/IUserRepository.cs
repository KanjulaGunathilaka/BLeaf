using BLeaf.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLeaf.Models.IRepository
{
    public interface IUserRepository
    {
        IEnumerable<User> AllUsers { get; }

        Task<User> SaveUser(User user);

        Task<User> UpdateUser(User user);

        Task<User> DeleteUser(int userId);

        Task<User> FindUserById(int userId);

        Task<User> FindUserByEmail(string email);
    }
}