using BusinessObjects;

namespace DataAccessLayers.Interface
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task AddToCartAsync(string userId, string? sellProductId = null, string? mangaBoxId = null);
        Task<Cart?> GetCartByUserIdAsync(string userId);
        Task<List<CartProduct>> GetCartProductsByCartIdAsync(string cartId);
        Task<List<CartBox>> GetCartBoxesByCartIdAsync(string cartId);
        Task RemoveFromCartAsync(string userId, string? sellProductId = null, string? mangaBoxId = null);
        Task ClearCartAsync(string userId);
    }
}