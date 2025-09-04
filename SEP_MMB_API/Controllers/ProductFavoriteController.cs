using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.Schema_Response;
using BusinessObjects.Dtos.UserCollection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductFavoriteController(IAuthService authService, IProductFavoriteService productFavoriteService)
        : ControllerBase
    {
        [Authorize]
        [HttpGet("get-image-product-favorite")]
        public async Task<ActionResult<ResponseModel<List<UserCollectionGetAllDto>>>> GetImageOfFavoriteListAsync()
        {
            try
            {
                var (account, _, _, _) = await authService.GetUserWithTokens(HttpContext);
                var response = await productFavoriteService.GetFavoriteListWithDetailsAsync(account.Id);

                return Ok(new ResponseModel<List<UserCollectionGetAllDto>>
                {
                    Success = true,
                    Data = response
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
        [Authorize]
        [HttpGet("get-all-product-favorite")]
        public async Task<ActionResult<ResponseModel<List<CollectionProductsDto>>>> GetAllAsync()
        {
            try
            {
                var (account, _, _, _) = await authService.GetUserWithTokens(HttpContext);
                var response = await productFavoriteService.GetAllWithDetailsAsync(account.Id);

                return Ok(new ResponseModel<List<CollectionProductsDto>>
                {
                    Success = true,
                    Data = response
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

        [Authorize]
        [HttpPost("add-product-favorite")]
        public async Task<ActionResult<ResponseModel<string>>> CreateProductFavorite(string userProductId)
        {
            try
            {
                var (account, _, _, _) = await authService.GetUserWithTokens(HttpContext);
                var response = await productFavoriteService.CreateAsync(account.Id, userProductId);
                if (!response)
                {
                    return BadRequest(new ResponseModel<string>
                    {
                        Success = true,
                        Data = "Add to favorite failed because this product exist in your favorite"
                    });
                }
                return Ok(new ResponseModel<string>
                {
                    Success = true,
                    Data = "Add to favorite successfull"
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

        [Authorize]
        [HttpDelete("remove-favorite-product")]
        public async Task<ActionResult<ResponseModel<string>>> DeleteProductFavorite(string favoriteId)
        {
            try
            {
                var response = await productFavoriteService.DeleteAsync(favoriteId);

                return Ok(new ResponseModel<string>
                {
                    Success = true,
                    Data = "Remove favorite successfull"
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
    }
}
