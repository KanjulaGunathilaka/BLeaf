using BLeaf.Models;
using System.Collections.Generic;

namespace BLeaf.ViewModels
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<ShoppingCartItem> Items { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DeliveryCharges { get; set; }
        public decimal Taxes { get; set; }
        public decimal GrandTotal { get; set; }
        public string UserId { get; set; }

        public ShoppingCartViewModel()
        {
            Items = new List<ShoppingCartItem>();
        }
    }
}