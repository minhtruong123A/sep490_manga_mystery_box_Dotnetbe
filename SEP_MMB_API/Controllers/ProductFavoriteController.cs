using BusinessObjects;
using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Core.Events;
using Services.Interface;
using Services.Service;


namespace SEP_MMB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductFavoriteController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IProductFavoriteService _productFavoriteService;

        public ProductFavoriteController(IAuthService authService, IProductFavoriteService productFavoriteService)
        {
            _authService = authService;
            _productFavoriteService = productFavoriteService;
        }

        [Authorize]
        [HttpGet("get-all-product-favorite")]
        public async Task<ActionResult<ResponseModel<List<CollectionProductsDto>>>> GetAllAsync()
        {
            try
            {
                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
                var response = await _productFavoriteService.GetAllWithDetailsAsync(account.Id);

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
                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
                var response = await _productFavoriteService.CreateAsync(account.Id, userProductId);

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
                var response = await _productFavoriteService.DeleteAsync(favoriteId);

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
