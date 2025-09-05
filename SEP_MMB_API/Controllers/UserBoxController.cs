using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.Schema_Response;
using BusinessObjects.Dtos.UserBox;
using BusinessObjects.Dtos.UserCollection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserBoxController(IUserBoxService userBoxService, IAuthService authService) : ControllerBase
    {
        [Authorize]
        [HttpGet("get-all-box-of-profile")]
        public async Task<ActionResult<ResponseModel<List<UserBoxGetAllDto>>>> GetAllCollectionOfProfile()
        {
            try
            {
                var (account, _, _, _) = await authService.GetUserWithTokens(HttpContext);

                var boxDto = await userBoxService.GetAllWithDetailsAsync(account.Id.ToString());

                return Ok(new ResponseModel<List<UserBoxGetAllDto>>
                {
                    Data = boxDto,
                    Error = null,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<List<UserCollectionGetAllDto>>
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 400
                });
            }
        }

        [Authorize(Roles = "user")]
        [HttpPost("open-box/{userBoxId}")]
        public async Task<ActionResult<ResponseModel<ProductResultDto>>> OpenBoxAsync(string userBoxId)
        {
            try
            {
                var (account, _, _, _) = await authService.GetUserWithTokens(HttpContext);
                var result = await userBoxService.OpenMysteryBoxAsync(userBoxId, account.Id.ToString());

                return Ok(new ResponseModel<ProductResultDto>
                {
                    Data = result,
                    Error = null,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<ProductResultDto>
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
