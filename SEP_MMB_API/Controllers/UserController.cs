using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Services;

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
        public async Task<ActionResult<List<User>>> Get()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        //[HttpGet("{id:length(24)}")]
        //public async Task<ActionResult<User>> Get(string id)
        //{
        //    var user = await _userService.GetUserByIdAsync(id);

        //    if (user == null)
        //        return NotFound();

        //    return Ok(user);
        //}

        [HttpPost]
        public async Task<ActionResult<User>> Create(User user)
        {
            await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }

        //[HttpPut("{id:length(24)}")]
        //public async Task<IActionResult> Update(string id, User user)
        //{
        //    var existingUser = await _userService.GetUserByIdAsync(id);

        //    if (existingUser == null)
        //        return NotFound();

        //    user.Id = id;
        //    await _userService.UpdateUserAsync(id, user);

        //    return NoContent();
        //}

        //[HttpDelete("{id:length(24)}")]
        //public async Task<IActionResult> Delete(string id)
        //{
        //    var user = await _userService.GetUserByIdAsync(id);

        //    if (user == null)
        //        return NotFound();

        //    await _userService.DeleteUserAsync(id);

        //    return NoContent();
        //}
    }
}
