using BusinessObjects.Dtos.Achievement;
using BusinessObjects.Dtos.Reward;
using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AchievementController(IAuthService authService, IAchievementService achievementService)
        : ControllerBase
    {
        [Authorize]
        [HttpGet("get-achievement-with-reward-of-collection")]
        public async Task<ActionResult<ResponseModel<AchievementWithAllRewardDto>>> GetAchievementWithRewardOfCollection(string collectionId)
        {
            try
            {
                var response = await achievementService.GetAchiementWithRewardByCollectionIdAsync(collectionId);
                if (response == null)
                {
                    return BadRequest(new ResponseModel<string>
                    {
                        Success = false,
                        Error = "Failed to load achievement",
                        ErrorCode = 400
                    });
                }
                return Ok(new ResponseModel<AchievementWithAllRewardDto>
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
        [HttpPost("Create-achievement-of-collection")]
        public async Task<ActionResult<ResponseModel<string>>> CreateAchievement(string collectionId, string name_Achievement)
        {
            try
            {
                var response = await achievementService.CreateAchievementOfCollection(collectionId, name_Achievement);
                if (!response)
                {
                    return BadRequest(new ResponseModel<string>
                    {
                        Success = false,
                        Error = "Reward box can't create achivement",
                        ErrorCode = 400
                    });
                }
                return Ok(new ResponseModel<string>
                {
                    Success = true,
                    Data = "Add achievement successfull"
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
        [HttpPost("Create-reward-of-achivement")]
        public async Task<ActionResult<ResponseModel<string>>> CreateReward(string collectionId,[FromForm] RewardCreateDto dto)
        {
            try
            {
                var response = await achievementService.CreateRewardOfAchievement(collectionId, dto);
                if (!response)
                {
                    return BadRequest(new ResponseModel<string>
                    {
                        Success = false,
                        Error = "Create reawrd failed",
                        ErrorCode = 400
                    });
                }
                return Ok(new ResponseModel<string>
                {
                    Success = true,
                    Data = "Add reaward successfull"
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
