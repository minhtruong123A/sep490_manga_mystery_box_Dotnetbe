using BusinessObjects;
using BusinessObjects.Dtos.Auth;
using BusinessObjects.Dtos.Schema_Response;
using BusinessObjects.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Service;

namespace SEP_MMB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ResponseModel<AuthResponseDto>>> Login([FromQuery] LoginDto loginDto)
        {
            try
            {
                var response = await _authService.Login(loginDto);
                return Ok(new ResponseModel<AuthResponseDto>()
                {
                    Data = response,
                    Error = null,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new ResponseModel<AuthResponseDto>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 401
                });
            }
        }

        [Authorize]
        [HttpGet("who-am-i")]
        public async Task<ActionResult<ResponseModel<UserTokenDto>>> WhoAmI()
        {
            try
            {
                var (account, accessToken, refreshToken, tokenType) = await _authService.GetUserWithTokens(HttpContext);

                return Ok(new ResponseModel<UserTokenDto>()
                {
                    Data = new UserTokenDto()
                    {
                        AccessToken = accessToken ?? "",
                        RefreshToken = refreshToken ?? "",
                        TokenType = tokenType ?? "",
                        UserName = account.Username ?? "",
                        RoleId = account.RoleId ?? "",
                    },
                    Error = null,
                    Success = true,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<User>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 401
                });
            }
        }
    }
}
