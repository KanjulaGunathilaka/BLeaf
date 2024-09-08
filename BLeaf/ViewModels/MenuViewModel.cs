using BLeaf.Models;

namespace BLeaf.ViewModels
{
    public class MenuViewModel
	{
		public IEnumerable<Category> Categories { get; set; }
		public IEnumerable<Item>? Items { get; set; }

        public MenuViewModel() { }

        public MenuViewModel(IEnumerable<Item>? items)
        {
            Items = items;
        }
    }
}
