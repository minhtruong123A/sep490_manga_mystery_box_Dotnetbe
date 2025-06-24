using BusinessObjects;
using BusinessObjects.Dtos.Auth;
using BusinessObjects.Dtos.Schema_Response;
using BusinessObjects.Dtos.User;
using DataAccessLayers.Exceptions;
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
        private readonly IUseDigitalWalletService _useDigitalWalletService;

        public AuthController(IAuthService authService, IUseDigitalWalletService useDigitalWalletService)
        {
            _authService = authService;
            _useDigitalWalletService = useDigitalWalletService;
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
                var walletAmount = await _useDigitalWalletService.GetWalletByIdAsync(account.WalletId);

                return Ok(new ResponseModel<UserTokenDto>()
                {
                    Data = new UserTokenDto()
                    {
                        AccessToken = accessToken ?? "",
                        TokenType = tokenType ?? "",
                        UserId = account.Id ?? "",
                        UserName = account.Username ?? "",
                        Amount = walletAmount?.Ammount ?? 0,
                        ProfileImage = account.ProfileImage ?? "",
                        RoleId = account.RoleId ?? "",
                    },
                    Error = null,
                    Success = true,
                });
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ResponseModel<object>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 403
                });
            }
            catch (Exception ex)
            {
                return StatusCode(400, new ResponseModel<User>()
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
