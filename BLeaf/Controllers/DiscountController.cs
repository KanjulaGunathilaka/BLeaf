using BLeaf.Models;
using BLeaf.Models.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLeaf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountRepository _discountRepository;

        public DiscountController(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Discount>>> GetAllDiscounts()
        {
            var discounts = _discountRepository.AllDiscounts;
            return Ok(discounts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Discount>> GetDiscountById(int id)
        {
            var discount = await _discountRepository.FindDiscountById(id);
            if (discount == null)
            {
                return NotFound();
            }
            return Ok(discount);
        }

        [HttpPost]
        public async Task<ActionResult<Discount>> AddDiscount([FromBody] Discount discount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdDiscount = await _discountRepository.SaveDiscount(discount);
            return CreatedAtAction(nameof(GetDiscountById), new { id = createdDiscount.DiscountId }, createdDiscount);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiscount(int id, [FromBody] Discount discount)
        {
            if (discount == null || discount.DiscountId != id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedDiscount = await _discountRepository.UpdateDiscount(discount);
                return Ok(updatedDiscount);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            try
            {
                var deletedDiscount = await _discountRepository.DeleteDiscount(id);
                return Ok(deletedDiscount);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("validate/{code}")]
        public async Task<ActionResult<Discount>> ValidatePromoCode(string code)
        {
            var discount = await _discountRepository.FindDiscountByCode(code);
            if (discount == null || (discount.IsActive.HasValue && !discount.IsActive.Value) ||
                (discount.ValidFrom.HasValue && discount.ValidFrom > DateTime.Now) ||
                (discount.ValidTo.HasValue && discount.ValidTo < DateTime.Now))
            {
                return NotFound();
            }
            return Ok(discount);
        }
    }
}