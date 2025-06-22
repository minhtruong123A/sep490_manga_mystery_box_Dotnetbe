using BusinessObjects.Dtos.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ICartService
    {
        Task AddToCartAsync(string userId, string? sellProductId = null, string? mangaBoxId = null);
        Task<CartViewDto> ViewCartAsync(string userId);
        Task RemoveFromCartAsync(string userId, string? sellProductId = null, string? mangaBoxId = null);
        Task ClearCartAsync(string userId);

    }
}
