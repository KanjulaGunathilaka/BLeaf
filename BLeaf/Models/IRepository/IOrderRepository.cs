using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLeaf.Models.IRepository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order> GetByIdAsync(int id);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(int id);
        Task<Item> GetItemByIdAsync(int itemId);
        Task<int> GetPendingOrdersCountAsync();
    }
}