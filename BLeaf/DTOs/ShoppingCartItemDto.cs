namespace BLeaf.DTOs
{
    public class ShoppingCartItemDto
    {
        public int ItemId { get; set; }
        public int? UserId { get; set; }
        public int Quantity { get; set; }
    }
}