using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.Schema_Response;
using BusinessObjects.Dtos.UserCollection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Service;

namespace SEP_MMB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProductController : ControllerBase
    {
        private readonly IUserProductService _userProductService;
        private readonly IAuthService _authService;
       public UserProductController(IUserProductService userProductService, IAuthService authService)
        {
            _userProductService = userProductService;
            _authService = authService;
        }

        [Authorize]
        [HttpGet("get-all-product-of-collection")]
        public async Task<ActionResult<ResponseModel<List<CollectionProductsDto>>>> GetAllProductOfCollection(string token, string collectionId)
        {
            try
            {
                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);

                var productsDto = await _userProductService.GetAllWithDetailsAsync(account.Id.ToString(), collectionId);

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
        [HttpPost("add-cards-to-collection")]
        public async Task<ActionResult<ResponseModel<string>>> AddCardsToOwnCollection([FromBody] AddCardsToCollectionDto dto)
        {
            try
            {
                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
                string userId = account.Id.ToString();

                await _userProductService.AddCardsToCollectionAsync(userId, dto.CollectionId, dto.ProductIds);

                return Ok(new ResponseModel<string>
                {
                    Data = "Cards added successfully",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<string>
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
