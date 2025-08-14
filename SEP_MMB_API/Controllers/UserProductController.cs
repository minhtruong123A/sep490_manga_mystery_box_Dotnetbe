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

        //check codex
        [Authorize]
        [HttpGet("get-all-product-of-user-collection")]
        public async Task<ActionResult<ResponseModel<List<CollectionProductsDto>>>> GetAllProductOfCollection(string collectionId)
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

    }
}
