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
    public class AchievementController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IAchievementService _achievementService;

        public AchievementController(IAuthService authService, IAchievementService achievementService)
        {
            _authService = authService;
            _achievementService = achievementService;
        }

        [Authorize]
        [HttpPost("Create-achievement-of-collection")]
        public async Task<ActionResult<ResponseModel<string>>> CreateAchievementWithReward(string collectionId, [FromForm]AchievementWithRewardsCreateDto dto)
        {
            try
            {
                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
                var response = await _achievementService.CreateAchievementWithRewardOfCollection(collectionId, dto);
                if (!response)
                {
                    return BadRequest(new ResponseModel<string>
                    {
                        Success = false,
                        Error = "Box reward can't create achivement",
                        ErrorCode = 400
                    });
                }
                return Ok(new ResponseModel<string>
                {
                    Success = true,
                    Data = "Add to favorite successfull"
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
