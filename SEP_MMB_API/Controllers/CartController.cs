using BusinessObjects.Dtos.Cart;
using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Service;

namespace SEP_MMB_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IAuthService _authService;

        public CartController(ICartService cartService, IAuthService authService)
        {
            _cartService = cartService;
            _authService = authService;
        }

        [Authorize(Roles = "user")]
        [HttpGet("view-cart")]
        public async Task<ActionResult<ResponseModel<CartViewDto>>> ViewCart()
        {
            try
            {
                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
                var cart = await _cartService.ViewCartAsync(account.Id);
                return Ok(new ResponseModel<CartViewDto>
                {
                    Data = cart,
                    Success = true,
                    Error = null,
                    ErrorCode = 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<CartViewDto>
                {
                    Data = null,
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }

        [Authorize(Roles = "user")]
        [HttpPost("add-to-cart")]
        public async Task<ActionResult<ResponseModel<object>>> AddToCart([FromQuery] AddToCartRequestDto request)
        {
            try
            {
                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
                await _cartService.AddToCartAsync(account.Id, request.SellProductId, request.MangaBoxId);
                return Ok(new ResponseModel<object>
                {
                    Data = new { message = "Item added to cart successfully." },
                    Success = true,
                    Error = null,
                    ErrorCode = 0
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseModel<object>
                {
                    Data = null,
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<object>
                {
                    Data = null,
                    Success = false,
                    Error = "error:" + ex.Message,
                    ErrorCode = 400
                });
            }
        }

        [Authorize(Roles = "user")]
        [HttpDelete("remove-from-cart")]
        public async Task<ActionResult<ResponseModel<object>>> RemoveFromCart([FromQuery] string? sellProductId, [FromQuery] string? mangaBoxId)
        {
            try
            {
                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
                if (string.IsNullOrWhiteSpace(sellProductId) && string.IsNullOrWhiteSpace(mangaBoxId))
                {
                    return BadRequest(new ResponseModel<object>
                    {
                        Success = false,
                        Error = "You must provide either sellProductId or mangaBoxId.",
                        ErrorCode = 400
                    });
                }

                await _cartService.RemoveFromCartAsync(account.Id, sellProductId, mangaBoxId);

                return Ok(new ResponseModel<object>
                {
                    Success = true,
                    Data = new { message = "Item removed from cart successfully." },
                    Error = null,
                    ErrorCode = 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<object>
                {
                    Data = null,
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }

        [Authorize(Roles = "user")]
        [HttpDelete("clear-all-cart")]
        public async Task<ActionResult<ResponseModel<object>>> ClearCart()
        {
            try
            {
                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
                await _cartService.ClearCartAsync(account.Id);

                return Ok(new ResponseModel<object>
                {
                    Success = true,
                    Data = new { message = "Cart cleared successfully." },
                    Error = null,
                    ErrorCode = 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<object>
                {
                    Data = null,
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }
    }
}
