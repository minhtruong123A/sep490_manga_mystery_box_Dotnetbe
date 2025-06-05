using BusinessObjects;
using BusinessObjects.Dtos.Schema_Response;
using BusinessObjects.Dtos.User;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
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


        // not fix yet
        //[HttpPost]
        //public async Task<ActionResult<User>> Create(User user)
        //{
        //    await _userService.CreateUserAsync(user);
        //    return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        //}
    }
}
