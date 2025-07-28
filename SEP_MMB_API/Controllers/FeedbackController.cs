using BusinessObjects;
using BusinessObjects.Dtos.Feedback;
using BusinessObjects.Dtos.MangaBox;
using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Service;

namespace SEP_MMB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IFeedbackService _feedbackService;
        public FeedbackController (IAuthService authService, IFeedbackService feedbackService)
        {
            _authService = authService;
            _feedbackService = feedbackService;
        }
        [Authorize]
        [HttpGet("Get-feedback-of-sell-product")]
        public async Task<ActionResult<ResponseModel<List<Feedback>>>> CreateFeedbackAsync(string sellProductID)
        {
            try
            {
                var response = await _feedbackService.GetAllFeedbackOfProductSaleAsync(sellProductID);
                if (response != null)
                {
                    return Ok(new ResponseModel<List<Feedback>>
                    {
                        Success = true,
                        Data = response,
                        Error = null,
                        ErrorCode = 0
                    });
                }
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Data = null,
                    Error = "Failed to load",
                    ErrorCode = 400
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

        [Authorize]
        [HttpPost("Create-feedback")]
        public async Task<ActionResult<ResponseModel<string>>> CreateFeedbackAsync([FromForm] FeedbackCreateDto dto)
        {
            try
            {
                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
                var response = await _feedbackService.CreateFeedbackAsync(account.Id,dto);
                if (response)
                {
                    return Ok(new ResponseModel<string>
                    {
                        Success = true,
                        Data = "Create feedback successfully!",
                        Error = null,
                        ErrorCode = 0
                    });
                }
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Data = null,
                    Error = "Failed to create",
                    ErrorCode = 400
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
