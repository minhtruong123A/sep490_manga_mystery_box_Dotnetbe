using BusinessObjects.Dtos.Cart;
using DataAccessLayers.Interface;
using DataAccessLayers.Repository;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISellProductService _sellProductService;
        private readonly IMangaBoxService _mangaBoxService;

        public CartService(IUnitOfWork unitOfWork, ISellProductService sellProductService, IMangaBoxService maaBoxService)
        {
            _unitOfWork = unitOfWork;
            _sellProductService = sellProductService;
            _mangaBoxService = maaBoxService;
        }

        public async Task AddToCartAsync(string userId, string? sellProductId = null, string? mangaBoxId = null)
        {
            await _unitOfWork.CartRepository.AddToCartAsync(userId, sellProductId, mangaBoxId);
        }

        public async Task<CartViewDto> ViewCartAsync(string userId)
        {
            var cart = await _unitOfWork.CartRepository.GetCartByUserIdAsync(userId);
            if (cart == null) throw new ArgumentException("Cart not found for user.");

            var cartProductItems = await _unitOfWork.CartRepository.GetCartProductsByCartIdAsync(cart.Id);
            var cartBoxItems = await _unitOfWork.CartRepository.GetCartBoxesByCartIdAsync(cart.Id);
            var productList = new List<CartProductDto>();
            foreach (var cartProduct in cartProductItems)
            {
                var detail = await _sellProductService.GetProductDetailByIdAsync(cartProduct.SellProductId);
                if (detail != null)
                {
                    productList.Add(new CartProductDto
                    {
                        SellProductId = cartProduct.SellProductId,
                        Product = detail,
                        Quantity = cartProduct.Quantity
                    });
                }
            }

            var boxList = new List<CartBoxDto>();
            foreach (var cartBox in cartBoxItems)
            {
                var detail = await _mangaBoxService.GetByIdWithDetailsAsync(cartBox.MangaBoxId);
                if (detail != null)
                {
                    boxList.Add(new CartBoxDto
                    {
                        MangaBoxId = cartBox.MangaBoxId,
                        Box = detail,
                        Quantity = cartBox.Quantity
                    });
                }
            }

            return new CartViewDto
            {
                Products = productList,
                Boxes = boxList
            };
        }

        public async Task RemoveFromCartAsync(string userId, string? sellProductId = null, string? mangaBoxId = null) => await _unitOfWork.CartRepository.RemoveFromCartAsync(userId, sellProductId, mangaBoxId);

        public async Task ClearCartAsync(string userId, string type) => await _unitOfWork.CartRepository.ClearCartAsync(userId, type);

        public async Task<UpdateCartItemDto> UpdateItemQuantityAsync(string userId, string itemId, int newQuantity)
        {
            var cart = await _unitOfWork.CartRepository.GetCartByUserIdAsync(userId);
            if (cart == null) throw new ArgumentException("Cart not found for this user.");

            var success = await _unitOfWork.CartRepository.UpdateItemQuantityAsync(cart.Id, itemId, newQuantity);
            if (!success) throw new ArgumentException("Item not found in the cart.");

            return new UpdateCartItemDto
            {
                Id = itemId,
                Quantity = newQuantity
            };
        }
    }
}