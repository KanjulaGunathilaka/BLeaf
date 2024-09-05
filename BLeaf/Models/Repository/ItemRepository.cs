using BLeaf.Data;
using BLeaf.Models;
using Microsoft.EntityFrameworkCore;
using BLeaf.Models.IRepository;

namespace BLeaf.Models.Repository
{
    public class ItemRepository : IItemRepository
	{
        private readonly ApplicationDbContext _applicationDbContext;

        //Press Alt+Enter to generate constructor
        public ItemRepository(ApplicationDbContext applicationDbContext)
        {
			_applicationDbContext = applicationDbContext;
        }

        public IEnumerable<Item> AllItems
        {
            get
            {
                return _applicationDbContext.Items.Include(c => c.Category);
            }

        }

        public IEnumerable<Item> PopularItemGifts
        {
            get
            {
                return _applicationDbContext.Items.Include(c => c.Category).Where(p => p.IsSpecial);
            }
        }

        public async Task<Item> GetItemById(int id)
        {
            return await _applicationDbContext.Items.FindAsync(id);
        }

        public async Task<Item> SaveItem(Item Item)
        {
            await _applicationDbContext.Categories.FindAsync(Item.CategoryId);
            _applicationDbContext.Items.Add(Item);
            await _applicationDbContext.SaveChangesAsync();
            return Item;
        }

        public async Task<Item> UpdateItem(Item Item)
        {
            var existingItem = await _applicationDbContext.Items.AsNoTracking().FirstOrDefaultAsync(p => p.ItemId == Item.ItemId);
            if (existingItem == null)
            {
                throw new KeyNotFoundException($"Item with ID {Item.ItemId} not found.");
            }

            _applicationDbContext.Entry(existingItem).State = EntityState.Detached;
            _applicationDbContext.Items.Update(Item);
            await _applicationDbContext.SaveChangesAsync();
            return Item;
        }

        public async Task<Item> DeleteItem(int ItemId)
        {
            var Item = await _applicationDbContext.Items.FindAsync(ItemId);
            if (Item == null)
            {
                throw new KeyNotFoundException($"Item with ID {ItemId} not found.");
            }

            _applicationDbContext.Items.Remove(Item);
            await _applicationDbContext.SaveChangesAsync();
            return Item;
        }

        public IEnumerable<Item> SearchItems(string searchQuery)
        {
            return _applicationDbContext.Items.Where(j => j.Name.Contains(searchQuery));
        }
    }
}
