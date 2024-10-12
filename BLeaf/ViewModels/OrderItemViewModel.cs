using BLeaf.Models;
using System.Collections.Generic;

namespace BLeaf.ViewModels
{
    public class OrderItemViewModel
    {
        public int ItemId { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderDetailViewModel
    {
        public OrderItemViewModel Item { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderViewModel
    {
        public User User { get; set; }
        public Address Address { get; set; }
        public decimal OrderTotal { get; set; }
        //public string PaymentMethod { get; set; }
        public List<OrderDetailViewModel> Items { get; set; }
    }
}