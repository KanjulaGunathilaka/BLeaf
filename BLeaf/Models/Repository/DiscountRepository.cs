using BLeaf.Data;
using BLeaf.Models.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLeaf.Models.Repository
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public DiscountRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IEnumerable<Discount> AllDiscounts => _applicationDbContext.Discounts.OrderBy(d => d.Code);

        public async Task<Discount> SaveDiscount(Discount discount)
        {
            _applicationDbContext.Discounts.Add(discount);
            await _applicationDbContext.SaveChangesAsync();
            return discount;
        }

        public async Task<Discount> UpdateDiscount(Discount discount)
        {
            _applicationDbContext.Discounts.Update(discount);
            await _applicationDbContext.SaveChangesAsync();
            return discount;
        }

        public async Task<Discount> DeleteDiscount(int discountId)
        {
            var discount = await _applicationDbContext.Discounts.FindAsync(discountId);
            if (discount == null)
            {
                throw new KeyNotFoundException($"Discount with ID {discountId} not found.");
            }

            _applicationDbContext.Discounts.Remove(discount);
            await _applicationDbContext.SaveChangesAsync();
            return discount;
        }

        public async Task<Discount> FindDiscountById(int discountId)
        {
            return await _applicationDbContext.Discounts.FindAsync(discountId);
        }

        public async Task<Discount> FindDiscountByCode(string code)
        {
            return await _applicationDbContext.Discounts.FirstOrDefaultAsync(d => d.Code == code);
        }
    }
}