using BusinessObjects.Dtos.Comment;
using BusinessObjects.Dtos.Schema_Response;
using DataAccessLayers.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IAuthService _authService;

        public CommentController(ICommentService commentService, IAuthService authService)
        {
            _commentService = commentService;
            _authService = authService;
        }

        [HttpGet("get-all-comment-by-sellproduct/{sellProductId}")]
        public async Task<ActionResult<ResponseModel<object>>> GetAllBySellProductId(string sellProductId)
        {
            var response = new ResponseModel<object>();
            try
            {
                var comments = await _commentService.GetAlCommentlBySellProductIdAsync(sellProductId);
                response.Success = true;
                response.Data = comments;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorCode = 400;
                return BadRequest(response);
            }
        }

        [HttpGet("get-all-rating-by-sellproduct/{sellProductId}")]
        public async Task<ActionResult<ResponseModel<object>>> GetAllratingBySellProductId(string sellProductId)
        {
            var response = new ResponseModel<object>();
            try
            {
                var ratings = await _commentService.GetAllRatingBySellProductIdAsync(sellProductId);
                response.Success = true;
                response.Data = ratings;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorCode = 400;
                return BadRequest(response);
            }
        }

        [Authorize]
        [HttpPost("create-comment")]
        public async Task<ActionResult<ResponseModel<object>>> CreateComment([FromBody] CommentCreateDto dto)
        {
            var response = new ResponseModel<object>();
            try
            {
                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
                var comment = await _commentService.CreateCommentAsync(dto.SellProductId, account.Id, dto.Content);
                response.Success = true;
                response.Data = comment;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorCode = 400;
                return BadRequest(response);
            }
        }

        [HttpPost("create-rating-only")]
        public async Task<ActionResult<ResponseModel<object>>> CreateRatingOnly([FromBody] CommentRatingCreateDto dto)
        {
            var response = new ResponseModel<object>();
            try
            {
                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
                var comment = await _commentService.CreateRatingOnlyAsync(dto.SellProductId, account.Id, dto.Rating);
                response.Success = true;
                response.Data = comment;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorCode = 400;
                return BadRequest(response);
            }
        }
    }
}