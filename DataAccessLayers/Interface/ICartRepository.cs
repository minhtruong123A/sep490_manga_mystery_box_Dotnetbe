﻿using BusinessObjects;

namespace DataAccessLayers.Interface
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task AddToCartAsync(string userId, string? sellProductId = null, string? mangaBoxId = null, int quantity = 1);
        Task<Cart?> GetCartByUserIdAsync(string userId);
        Task<List<CartProduct>> GetCartProductsByCartIdAsync(string cartId);
        Task<List<CartBox>> GetCartBoxesByCartIdAsync(string cartId);
        Task RemoveFromCartAsync(string userId, string? sellProductId = null, string? mangaBoxId = null);
        Task ClearCartAsync(string userId, string type);
        Task<bool> UpdateItemQuantityAsync(string cartId, string itemId, int quantity);
    }
}