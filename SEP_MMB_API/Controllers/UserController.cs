using BusinessObjects;
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
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        public UserController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

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
        public async Task<ActionResult<ResponseModel<List<UserInformationDto>>>> GetByToken(string token)
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

        [Authorize]
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
    }
}
