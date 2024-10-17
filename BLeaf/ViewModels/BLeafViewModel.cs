using BLeaf.Models;
using System.Collections.Generic;

namespace BLeaf.ViewModels
{
    public class BLeafViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Item> Items { get; set; }
        public IEnumerable<User> Users { get; set; }
        public List<string> Roles { get; set; }
        public IEnumerable<Discount> Discounts { get; set; }

        public BLeafViewModel()
        {
            Categories = new List<Category>();
            Items = new List<Item>();
            Users = new List<User>();
        }

        public BLeafViewModel(IEnumerable<Category> categories, IEnumerable<Item> items, IEnumerable<User> users, IEnumerable<Discount> discounts)
        {
            Categories = categories;
            Items = items;
            Users = users;
            Discounts = discounts;
        }
    }
}