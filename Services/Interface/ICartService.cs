using BusinessObjects.Dtos.Cart;

namespace Services.Interface;

public interface ICartService
{
    Task AddToCartAsync(string userId, string? sellProductId = null, string? mangaBoxId = null, int quantity = 1);
    Task<CartViewDto> ViewCartAsync(string userId);
    Task RemoveFromCartAsync(string userId, string? sellProductId = null, string? mangaBoxId = null);
    Task ClearCartAsync(string userId, string type);
    Task<UpdateCartItemDto> UpdateItemQuantityAsync(string userId, string itemId, int newQuantity);
}