using BLeaf.Models;

namespace BLeaf.ViewModels
{
    public class BLeafViewModel
    {
        public IEnumerable<Item>? Items { get; set; }

        public BLeafViewModel() { }

        public BLeafViewModel(IEnumerable<Item>? items)
        {
            Items = items;
        }
    }
}
