using BusinessObjects.Dtos.Cart;
using DataAccessLayers.Interface;
using Services.Interface;

namespace Services.Service;

public class CartService(IUnitOfWork unitOfWork, ISellProductService sellProductService, IMangaBoxService maaBoxService)
    : ICartService
{
    public async Task AddToCartAsync(string userId, string? sellProductId = null, string? mangaBoxId = null,
        int quantity = 1)
    {
        await unitOfWork.CartRepository.AddToCartAsync(userId, sellProductId, mangaBoxId, quantity);
    }

    public async Task<CartViewDto> ViewCartAsync(string userId)
    {
        var cart = await unitOfWork.CartRepository.GetCartByUserIdAsync(userId);
        if (cart == null) throw new ArgumentException("Cart not found for user.");

        var cartProductItems = await unitOfWork.CartRepository.GetCartProductsByCartIdAsync(cart.Id);
        var cartBoxItems = await unitOfWork.CartRepository.GetCartBoxesByCartIdAsync(cart.Id);
        var productList = new List<CartProductDto>();
        foreach (var cartProduct in cartProductItems)
        {
            var detail = await sellProductService.GetProductDetailByIdAsync(cartProduct.SellProductId);
            if (detail != null)
                productList.Add(new CartProductDto
                {
                    CartProductId = cartProduct.Id,
                    SellProductId = cartProduct.SellProductId,
                    Product = detail,
                    Quantity = cartProduct.Quantity
                });
        }

        var boxList = new List<CartBoxDto>();
        foreach (var cartBox in cartBoxItems)
        {
            var detail = await maaBoxService.GetByIdWithDetailsAsync(cartBox.MangaBoxId);
            if (detail != null)
                boxList.Add(new CartBoxDto
                {
                    CartBoxId = cartBox.Id,
                    MangaBoxId = cartBox.MangaBoxId,
                    Box = detail,
                    Quantity = cartBox.Quantity
                });
        }

        return new CartViewDto
        {
            Products = productList,
            Boxes = boxList
        };
    }

    public async Task RemoveFromCartAsync(string userId, string? sellProductId = null, string? mangaBoxId = null)
    {
        await unitOfWork.CartRepository.RemoveFromCartAsync(userId, sellProductId, mangaBoxId);
    }

    public async Task ClearCartAsync(string userId, string type)
    {
        await unitOfWork.CartRepository.ClearCartAsync(userId, type);
    }

    public async Task<UpdateCartItemDto> UpdateItemQuantityAsync(string userId, string itemId, int newQuantity)
    {
        var cart = await unitOfWork.CartRepository.GetCartByUserIdAsync(userId);
        if (cart == null) throw new ArgumentException("Cart not found for this user.");

        var success = await unitOfWork.CartRepository.UpdateItemQuantityAsync(cart.Id, itemId, newQuantity);
        if (!success) throw new ArgumentException("Item not found in the cart.");

        return new UpdateCartItemDto
        {
            Id = itemId,
            Quantity = newQuantity
        };
    }
}