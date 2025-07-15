using BusinessObjects;
using BusinessObjects.Dtos.Schema_Response;
using BusinessObjects.Dtos.User;
using BusinessObjects.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Service;

namespace SEP_MMB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        public UserController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        [HttpGet("get-all-accounts")]
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

        [Authorize]
        [HttpGet("get-profile")]
        public async Task<ActionResult<ResponseModel<List<UserInformationDto>>>> GetByToken()
        {
            try
            {
                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
                var userDto = await _userService.GetUserByIdAsync(account.Id);
                return Ok(new ResponseModel<UserInformationDto>()
                {
                    Data = userDto,
                    Error = null,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<UserInformationDto>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 400
                });
            }
        }

        [HttpGet("get-other-profile")]
        public async Task<ActionResult<ResponseModel<List<UserInformationDto>>>> GetById(string id)
        {
            try
            {
                var userDto = await _userService.GetUserByIdAsync(id);
                return Ok(new ResponseModel<UserInformationDto>()
                {
                    Data = userDto,
                    Error = null,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<UserInformationDto>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 400
                });
            }
        }

        [Authorize(Roles = "user")]
        [HttpPut("profile/change-password")]
        public async Task<ActionResult<ResponseModel<string>>> ChangePasswordAsync(ChangePasswordDto dto)
        {
            try
            {
                var result = await _userService.ChangePasswordAsync(dto);

                switch (result)
                {
                    case ChangePasswordResult.Success:
                        return Ok(new ResponseModel<string>
                        {
                            Data = "Password changed successfully",
                            Success = true
                        });

                    case ChangePasswordResult.PasswordMismatch:
                        return BadRequest(new ResponseModel<string>
                        {
                            Error = "New password and confirm password do not match",
                            ErrorCode = 1001,
                            Success = false
                        });

                    case ChangePasswordResult.InvalidCurrentPassword:
                        return BadRequest(new ResponseModel<string>
                        {
                            Error = "Current password is incorrect",
                            ErrorCode = 1002,
                            Success = false
                        });

                    default:
                        return BadRequest(new ResponseModel<string>
                        {
                            Error = "Unknown error",
                            ErrorCode = 1003,
                            Success = false
                        });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<string>
                {
                    Error = ex.Message,
                    ErrorCode = 400,
                    Success = false
                });
            }
        }

    }
}
