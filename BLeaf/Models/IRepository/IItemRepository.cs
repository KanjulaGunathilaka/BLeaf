using BLeaf.Models;
using System.IO.Pipelines;

namespace BLeaf.Models.IRepository
{
    public interface IItemRepository
    {
        IEnumerable<Item> AllItems { get; }

        IEnumerable<Item> PopularItemGifts { get; }

        Task<Item> GetItemById(int id);

        IEnumerable<Item> SearchItems(string searchQuery);

        Task<Item> SaveItem(Item Item);

        Task<Item> UpdateItem(Item Item);

        Task<Item> DeleteItem(int ItemId);
    }
}
