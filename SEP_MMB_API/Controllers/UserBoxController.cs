using Microsoft.AspNetCore.Mvc;
using BusinessObjects;
using BusinessObjects.Dtos.Schema_Response;
using BusinessObjects.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Service;
using BusinessObjects.Dtos.UserCollection;
using BusinessObjects.Dtos.UserBox;

namespace SEP_MMB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserBoxController : ControllerBase
    {
        private readonly IUserBoxService _userBoxService;
        private readonly IAuthService _authService;

        public UserBoxController(IUserBoxService userBoxService, IAuthService authService)
        {
            _userBoxService = userBoxService;
            _authService = authService;
        }

        [Authorize]
        [HttpGet("get-all-box-of-profile")]
        public async Task<ActionResult<ResponseModel<List<UserBoxGetAllDto>>>> GetAllCollectionOfProfile(string token)
        {
            try
            {
                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);

                var boxDto = await _userBoxService.GetAllWithDetailsAsync(account.Id.ToString());

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
    }
}
