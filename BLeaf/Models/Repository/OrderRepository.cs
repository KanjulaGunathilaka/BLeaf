using BLeaf.Data;
using BLeaf.Models.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLeaf.Models.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Item)
                .ToListAsync();
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Item)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task AddAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Item> GetItemByIdAsync(int itemId)
        {
            return await _context.Items.FindAsync(itemId);
        }

        public async Task<int> GetPendingOrdersCountAsync()
        {
            return await _context.Orders.CountAsync(o => o.OrderStatus == "Pending");
        }
    }
}