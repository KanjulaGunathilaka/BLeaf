using BLeaf.Models;
using System.IO.Pipelines;
using System.Linq;

namespace BLeaf.Models.IRepository
{
    public interface IItemRepository
    {
        IQueryable<Item> AllItems { get; }

        IQueryable<Item> PopularItemGifts { get; }

        Task<Item> GetItemById(int id);

        IEnumerable<Item> SearchItems(string searchQuery);

        Task<Item> SaveItem(Item Item);

        Task<Item> UpdateItem(Item Item);

        Task<Item> DeleteItem(int ItemId);
    }
}
