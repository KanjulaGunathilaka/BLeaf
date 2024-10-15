using Microsoft.AspNetCore.Mvc;
using BLeaf.Models;
using BLeaf.Models.IRepository;
using BLeaf.ViewModels;
using BLeaf.DTOs;

namespace BLeaf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartItemController : ControllerBase
    {
        private readonly IShoppingCartItemRepository _repository;
        private readonly IUserRepository _userRepository;

        public ShoppingCartItemController(IShoppingCartItemRepository repository, IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ShoppingCartViewModel>> GetShoppingCartItems()
        {
            var items = await _repository.GetAllAsync();
            var totalPrice = items.Sum(item => item.Item.Price * item.Quantity);
            var deliveryCharges = 0.00m; // Example value, you can calculate based on your logic
            var taxes = totalPrice * 0.00m; // Example tax calculation
            var grandTotal = totalPrice + deliveryCharges + taxes;

            var viewModel = new ShoppingCartViewModel
            {
                Items = items,
                TotalPrice = totalPrice,
                DeliveryCharges = deliveryCharges,
                Taxes = taxes,
                GrandTotal = grandTotal
            };

            return Ok(viewModel);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShoppingCartItem>> GetShoppingCartItem(int id)
        {
            var shoppingCartItem = await _repository.GetByIdAsync(id);

            if (shoppingCartItem == null)
            {
                return NotFound();
            }

            return shoppingCartItem;
        }

        [HttpPost]
        public async Task<ActionResult<ShoppingCartItem>> PostShoppingCartItem([FromBody] ShoppingCartItemDto shoppingCartItemDto)
        {
            if (shoppingCartItemDto == null)
            {
                return BadRequest("Invalid data.");
            }

            int? userId = shoppingCartItemDto.UserId;

            if (HttpContext.Session.GetString("UserSessionID") == null)
            {
                var guestUserEmail = $"guest_{Guid.NewGuid()}@example.com";

                var guestUser = new User
                {
                    FullName = "Guest",
                    Email = guestUserEmail,
                    PasswordHash = Guid.NewGuid().ToString()
                };

                var user = await _userRepository.SaveUser(guestUser);
                userId = user.UserId;

                HttpContext.Session.SetString("UserSessionID", guestUserEmail);
            }
            else
            {
                var user = await _userRepository.FindUserByEmail(HttpContext.Session.GetString("UserSessionID"));
                userId = user.UserId;
            }

            var shoppingCartItem = new ShoppingCartItem
            {
                ItemId = shoppingCartItemDto.ItemId,
                UserId = userId,
                Quantity = shoppingCartItemDto.Quantity
            };

            await _repository.AddAsync(shoppingCartItem);
            return CreatedAtAction("GetShoppingCartItem", new { id = shoppingCartItem.ShoppingCartItemId }, shoppingCartItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutShoppingCartItem(int id, ShoppingCartItem shoppingCartItem)
        {
            if (id != shoppingCartItem.ShoppingCartItemId)
            {
                return BadRequest();
            }

            await _repository.UpdateAsync(shoppingCartItem);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShoppingCartItem(int id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}