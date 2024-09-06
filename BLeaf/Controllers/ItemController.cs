using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BLeaf.Models;
using BLeaf.Models.IRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLeaf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemRepository _itemRepository;

        public ItemController(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetAllItems()
        {
            var items = await _itemRepository.AllItems.ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItemById(int id)
        {
            var item = await _itemRepository.GetItemById(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        //[HttpPost]
        //public async Task<ActionResult<Item>> AddItem([FromBody] Item item)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        // Log the model state errors for debugging
        //        foreach (var state in ModelState)
        //        {
        //            foreach (var error in state.Value.Errors)
        //            {
        //                Console.WriteLine($"Key: {state.Key}, Error: {error.ErrorMessage}");
        //            }
        //        }
        //        return BadRequest(ModelState);
        //    }

        //    var createdItem = await _itemRepository.SaveItem(item);
        //    return CreatedAtAction(nameof(GetItemById), new { id = createdItem.ItemId }, createdItem);
        //}

        [HttpPost]
        public async Task<ActionResult<Item>> AddItem([FromBody] Item item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _itemRepository.SaveItem(item);
            return CreatedAtAction(nameof(GetItemById), new { id = createdItem.ItemId }, createdItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] Item item)
        {
            if (item == null || item.ItemId != id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedItem = await _itemRepository.UpdateItem(item);
                return Ok(updatedItem);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                var deletedItem = await _itemRepository.DeleteItem(id);
                return Ok(deletedItem);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}