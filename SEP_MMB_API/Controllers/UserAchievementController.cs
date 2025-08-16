using BusinessObjects;
using BusinessObjects.Dtos.Achievement;
using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Service;

namespace SEP_MMB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAchievementController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IAchievementService _achievementService;
        public UserAchievementController(IAuthService authService, IAchievementService achievementService)
        {
            _authService = authService;
            _achievementService = achievementService;
        }
        [Authorize(Roles ="user")]
        [HttpGet("get-all-medal-of-user")]
        public async Task<IActionResult> GetAllMedalOfUsersAsync()
        {
            try
            {
                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
                var data = await _achievementService.GetAllMedalOfUserAsync(account.Id);
                if (data == null)
                {
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Data = null,
                        Error = "Don't have any report",
                        ErrorCode = 404
                    });
                }

                return Ok(new ResponseModel<List<GetAchievementMedalRewardDto>>
                {
                    Success = true,
                    Data = data,
                    Error = null,
                    ErrorCode = 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Data = null,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }
        [Authorize(Roles ="user")]
        [HttpGet("get-all-medal-public-of-user")]
        public async Task<IActionResult> GetAllMeadalPublicOfUserAsync(string userId)
        {
            try
            {
                var data = await _achievementService.GetAllMedalPublicOfUserAsync(userId);
                if (data == null)
                {
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Data = null,
                        Error = "Don't have any report",
                        ErrorCode = 404
                    });
                }

                return Ok(new ResponseModel<List<GetAchievementMedalRewardDto>>
                {
                    Success = true,
                    Data = data,
                    Error = null,
                    ErrorCode = 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Data = null,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }
        [Authorize(Roles ="user")]
        [HttpGet("get-user-collection-completion-achievement-progress")]
        public async Task<IActionResult> GetUserCollectionCompletionProgressAsync()
        {
            try
            {
                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
                var data = await _achievementService.GetUserCollectionCompletionProgressAsync(account.Id);
                if (data == null)
                {
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Data = null,
                        Error = "Don't have any report",
                        ErrorCode = 404
                    });
                }

                return Ok(new ResponseModel<List<AchievementOfUserCollectionCompletionProgressDto>>
                {
                    Success = true,
                    Data = data,
                    Error = null,
                    ErrorCode = 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Data = null,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }
        [Authorize(Roles ="user")]
        [HttpPatch("private-or-public-medal-of-user")]
        public async Task<IActionResult> ChangePrivateOrPublicMedalAsync(string userRewardId)
        {
            try
            {
                var data = await _achievementService.ChangePublicOrPrivateAsync(userRewardId);
                if (data == false)
                {
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Data = null,
                        Error = "Failed to change stattus",
                        ErrorCode = 400
                    });
                }

                return Ok(new ResponseModel<string>
                {
                    Success = true,
                    Data = "Change status successfull",
                    Error = null,
                    ErrorCode = 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Data = null,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }
    }
}
