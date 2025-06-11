using BusinessObjects.Dtos;
using BusinessObjects.Dtos.Auth;
using BusinessObjects.Dtos.Schema_Response;
using BusinessObjects.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Service;
using System.IdentityModel.Tokens.Jwt;

namespace SEP_MMB_API.Controllers
{
    [ApiExplorerSettings(GroupName = "test")]
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public TestController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [Tags("Server Test Fetch API Only")]
        [HttpGet("user")]
        public async Task<ActionResult<ResponseModel<List<UserInformationDto>>>> Get()
        {
            try
            {
                var usersDto = await _userService.GetAllUsersAsync();
                return Ok(new ResponseModel<List<UserInformationDto>>()
                {
                    Data = usersDto,
                    Error = null,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<List<UserInformationDto>>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 400
                });
            }
        }

        [Tags("Server Test Fetch API Only")]
        [HttpDelete("delete-by-email")]
        public async Task<ActionResult<ResponseModel<string>>> DeleteByEmail(string email)
        {
            try
            {
                await _userService.DeleteUserByEmailAsync(email);
                return Ok(new ResponseModel<string>
                {
                    Data = "User and associated email verification deleted successfully.",
                    Success = true,
                    Error = null
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

        [Tags("Server Test Fetch API Only")]
        [Authorize]
        [HttpGet("check-token-expiry")]
        public ActionResult<ResponseModel<BoolWrapper>> CheckTokenExpiry()
        {
            try
            {
                var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();

                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                    return Unauthorized(new ResponseModel<BoolWrapper>
                    {
                        Data = null,
                        Success = false,
                        Error = "Missing or invalid Authorization header",
                        ErrorCode = 401
                    });

                var token = authHeader.Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();
                
                handler.ValidateToken(token, _authService.GetValidationParameters(), out _);

                return Ok(new ResponseModel<BoolWrapper>
                {
                    Data = new BoolWrapper { Value = true },
                    Success = true,
                    Error = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(400, new ResponseModel<BoolWrapper>
                {
                    Data = null,
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }

        [Tags("Server Test Fetch API Only")]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<ResponseModel<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(dto.Token);

                return Ok(new ResponseModel<AuthResponseDto>
                {
                    Data = result,
                    Success = true,
                    Error = null,
                    ErrorCode = 0
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ResponseModel<AuthResponseDto>
                {
                    Data = null,
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 401
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<AuthResponseDto>
                {
                    Data = null,
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 500
                });
            }
        }
    }
}
