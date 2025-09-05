using BusinessObjects.Dtos.Cart;
using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController(ICartService cartService, IAuthService authService) : ControllerBase
    {
        [Authorize(Roles = "user")]
        [HttpGet("view-cart")]
        public async Task<ActionResult<ResponseModel<CartViewDto>>> ViewCart()
        {
            try
            {
                var (account, _, _, _) = await authService.GetUserWithTokens(HttpContext);
                var cart = await cartService.ViewCartAsync(account.Id);
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
                var (account, _, _, _) = await authService.GetUserWithTokens(HttpContext);
                var quantity = request.Quantity ?? 1;
                await cartService.AddToCartAsync(account.Id, request.SellProductId, request.MangaBoxId, quantity);
                return Ok(new ResponseModel<object>
                {
                    Data = new
                    {
                        Request = request,
                        message = "Item added to cart successfully."
                    },
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
        [HttpPut("update-quantity")]
        public async Task<ActionResult<ResponseModel<object>>> UpdateQuantity([FromBody] UpdateCartItemDto request)
        {
            try
            {
                var (account, _, _, _) = await authService.GetUserWithTokens(HttpContext);
                if (request.Quantity < 0) return BadRequest(new ResponseModel<object> { Success = false, Error = "Quantity cannot be negative." });

                var updatedItem = await cartService.UpdateItemQuantityAsync(account.Id, request.Id, request.Quantity);
                return Ok(new ResponseModel<UpdateCartItemDto>
                {
                    Data = updatedItem,
                    Success = true
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseModel<object>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 404
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ResponseModel<object>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
            catch (Exception ex)
            {
                return StatusCode(400, new ResponseModel<object>
                {
                    Success = false,
                    Error = ex.Message,
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
                var (account, _, _, _) = await authService.GetUserWithTokens(HttpContext);
                if (string.IsNullOrWhiteSpace(sellProductId) && string.IsNullOrWhiteSpace(mangaBoxId))
                {
                    return BadRequest(new ResponseModel<object>
                    {
                        Success = false,
                        Error = "You must provide either sellProductId or mangaBoxId.",
                        ErrorCode = 400
                    });
                }

                await cartService.RemoveFromCartAsync(account.Id, sellProductId, mangaBoxId);

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
        public async Task<ActionResult<ResponseModel<object>>> ClearCart([FromQuery] string type = "all")
        {
            try
            {
                var (account, _, _, _) = await authService.GetUserWithTokens(HttpContext);
                await cartService.ClearCartAsync(account.Id, type);

                return Ok(new ResponseModel<object>
                {
                    Success = true,
                    Data = new { message = $"Cart cleared ({type}) successfully." },
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
