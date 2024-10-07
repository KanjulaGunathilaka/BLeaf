using BLeaf.Models;
using System.Collections.Generic;

namespace BLeaf.ViewModels
{
    public class BLeafViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Item> Items { get; set; }
        public IEnumerable<User> Users { get; set; }

        public BLeafViewModel()
        {
            Categories = new List<Category>();
            Items = new List<Item>();
            Users = new List<User>();
        }

        public BLeafViewModel(IEnumerable<Category> categories, IEnumerable<Item> items, IEnumerable<User> users)
        {
            Categories = categories;
            Items = items;
            Users = users;
        }
    }
}