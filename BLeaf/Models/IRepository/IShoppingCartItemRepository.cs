using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLeaf.Models.IRepository
{
    public interface IShoppingCartItemRepository
    {
        Task<IEnumerable<ShoppingCartItem>> GetAllAsync();
        Task<ShoppingCartItem> GetByIdAsync(int id);
        Task AddAsync(ShoppingCartItem entity);
        Task UpdateAsync(ShoppingCartItem entity);
        Task DeleteAsync(int id);
    }
}