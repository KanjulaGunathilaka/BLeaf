using BLeaf.Data;
using BLeaf.Models.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLeaf.Models.Repository
{
    public class ShoppingCartItemRepository : IShoppingCartItemRepository
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ShoppingCartItem>> GetAllAsync()
        {
            return await _context.ShoppingCartItems.Include(sci => sci.User).Include(sci => sci.Item).ToListAsync();
        }

        public async Task<ShoppingCartItem> GetByIdAsync(int id)
        {
            return await _context.ShoppingCartItems.Include(sci => sci.User).Include(sci => sci.Item).FirstOrDefaultAsync(sci => sci.ShoppingCartItemId == id);
        }

        public async Task AddAsync(ShoppingCartItem entity)
        {
            var existingItem = await _context.ShoppingCartItems
                .FirstOrDefaultAsync(sci => sci.UserId == entity.UserId && sci.ItemId == entity.ItemId);

            if (existingItem != null)
            {
                existingItem.Quantity += entity.Quantity;
                _context.Entry(existingItem).State = EntityState.Modified;
            }
            else
            {
                _context.ShoppingCartItems.Add(entity);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ShoppingCartItem entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.ShoppingCartItems.FindAsync(id);
            if (entity != null)
            {
                _context.ShoppingCartItems.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}