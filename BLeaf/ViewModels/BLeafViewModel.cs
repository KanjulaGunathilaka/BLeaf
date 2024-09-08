using BLeaf.Models;
using System.Collections.Generic;

namespace BLeaf.ViewModels
{
	public class BLeafViewModel
	{
		public IEnumerable<Category> Categories { get; set; }
		public IEnumerable<Item> Items { get; set; }

		public BLeafViewModel()
		{
			Categories = new List<Category>();
			Items = new List<Item>();
		}

		public BLeafViewModel(IEnumerable<Category> categories, IEnumerable<Item> items)
		{
			Categories = categories;
			Items = items;
		}
	}
}