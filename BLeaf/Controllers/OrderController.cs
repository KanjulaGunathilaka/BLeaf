using BLeaf.Models;
using BLeaf.Models.IRepository;
using BLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLeaf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return Ok(await _orderRepository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] OrderViewModel orderViewModel)
        {
            if (orderViewModel == null || !orderViewModel.Items.Any())
            {
                return BadRequest("Invalid order data.");
            }

            var order = new Order
            {
                User = orderViewModel.User,
                Address = orderViewModel.Address,
                OrderTotal = orderViewModel.OrderTotal,
                OrderStatus = "Pending",
                PaymentStatus = "Unpaid",
                OrderPlacedAt = DateTime.Now,
                OrderDetails = orderViewModel.Items.Select(i => new OrderDetail
                {
                    ItemId = i.Item.ItemId,
                    Quantity = i.Quantity
                }).ToList()
            };

            await _orderRepository.AddAsync(order);

            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
        {
            if (id != order.OrderId)
            {
                return BadRequest("Order ID mismatch.");
            }

            await _orderRepository.UpdateAsync(order);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            await _orderRepository.DeleteAsync(id);

            return NoContent();
        }
    }
}