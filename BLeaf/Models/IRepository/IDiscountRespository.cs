using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLeaf.Models.IRepository
{
    public interface IDiscountRepository
    {
        IEnumerable<Discount> AllDiscounts { get; }
        Task<Discount> SaveDiscount(Discount discount);
        Task<Discount> UpdateDiscount(Discount discount);
        Task<Discount> DeleteDiscount(int discountId);
        Task<Discount> FindDiscountById(int discountId);
        Task<Discount> FindDiscountByCode(string code);
    }
}