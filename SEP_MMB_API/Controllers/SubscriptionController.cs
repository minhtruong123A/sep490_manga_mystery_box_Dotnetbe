using BusinessObjects.Dtos.Schema_Response;
using BusinessObjects.Dtos.Subscription;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController(IAuthService authService, ISubscriptionService subscriptionService)
        : ControllerBase
    {
        [Authorize]
        [HttpPost("subscription/add-follower")]
        public async Task<ActionResult<ResponseModel<string>>> CreateAsync([FromBody] SubscriptionCreateDto dto)
        {
            try
            {
                var (account, _, _, _) = await authService.GetUserWithTokens(HttpContext);
                var response = await subscriptionService.CreateAsync(account.Id,dto);

                return Ok(new ResponseModel<string>
                {
                    Success = true,
                    Data = "Add follwer sucessfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(400, new ResponseModel<object>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }

        //[Authorize]
        [HttpGet("subscription/get-all-followers")]
        public async Task<ActionResult<ResponseModel<List<SubcriptionFollowerResponeDto>>>> GetAllFollower(string myUserId)
        {
            try
            {
                //var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
                var response = await subscriptionService.GetAllFollowerOfUserAsync(myUserId);

                return Ok(new ResponseModel<List<SubcriptionFollowerResponeDto>>
                {
                    Success = true,
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(400, new ResponseModel<object>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }
        //[Authorize]
        [HttpGet("subscription/get-all-following")]
        public async Task<ActionResult<ResponseModel<List<SubcriptionFollowResponeDto>>>> GetAllFollow(string myUserId)
        {
            try
            {
                //var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
                var response = await subscriptionService.GetAllFollowOfUserAsync(myUserId);

                return Ok(new ResponseModel<List<SubcriptionFollowResponeDto>>
                {
                    Success = true,
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(400, new ResponseModel<object>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }

        [Authorize]
        [HttpDelete("subscription/unfollow")]
        public async Task<ActionResult<ResponseModel<string>>> Unfollow(string userId)
        {
            try
            {
                var (account, _, _, _) = await authService.GetUserWithTokens(HttpContext);
                var response = await subscriptionService.UnfollowAsync(userId,account.Id);
                if (response) {
                    return Ok(new ResponseModel<string>
                    {
                        Success = true,
                        Data = "Unfollow succesfull"
                    });
                }
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Data = "Unfollow failed"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(400, new ResponseModel<object>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }
    }
}
