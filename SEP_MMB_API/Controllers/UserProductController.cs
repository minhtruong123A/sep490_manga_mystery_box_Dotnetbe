using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProductController(IUserProductService userProductService, IAuthService authService)
        : ControllerBase
    {
        [Authorize]
        [HttpGet("get-all-product-of-user-collection")]
        public async Task<ActionResult<ResponseModel<List<CollectionProductsDto>>>> GetAllProductOfCollection(string collectionId)
        {
            try
            {
                var (account, _, _, _) = await authService.GetUserWithTokens(HttpContext);

                var productsDto = await userProductService.GetAllWithDetailsAsync(account.Id.ToString(), collectionId);

                return Ok(new ResponseModel<List<CollectionProductsDto>>
                {
                    Data = productsDto,
                    Error = null,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<List<CollectionProductsDto>>
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 400
                });
            }
        }
        [Authorize]
        [HttpPatch("checked-new-update-quantity-user-product")]
        public async Task<ActionResult<ResponseModel<string>>> CheckedNewUpdateQuantityUserProduct(string userProductId)
        {
            try
            {
                var productsDto = await userProductService.CheckedUpdateQuantityAsync(userProductId);

                return Ok(new ResponseModel<string>
                {
                    Data = "Checked successfull",
                    Error = null,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<string>
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 400
                });
            }
        }

    }
}
