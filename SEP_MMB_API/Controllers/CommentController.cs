using BusinessObjects.Dtos.Comment;
using BusinessObjects.Dtos.Schema_Response;
using DataAccessLayers.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController(ICommentService commentService, IAuthService authService) : ControllerBase
    {
        [HttpGet("get-all-badwords")]
        public async Task<ActionResult<ResponseModel<object>>> GetAllBadWords()
        {
            var response = new ResponseModel<object>();
            try
            {
                var badWords = await commentService.GetAllBadWordsAsync();
                response.Success = true;
                response.Data = badWords;
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

        //[HttpGet("get-all-allow-words")]
        //public async Task<ActionResult<ResponseModel<object>>> GetAllAllowWords()
        //{
        //    var response = new ResponseModel<object>();
        //    try
        //    {
        //        var badWords = await _commentService.GetAllAllowedShortWordsAsync();
        //        response.Success = true;
        //        response.Data = badWords;
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Success = false;
        //        response.Error = ex.Message;
        //        response.ErrorCode = 400;
        //        return BadRequest(response);
        //    }
        //}

        [HttpGet("get-all-comment-by-sellproduct/{sellProductId}")]
        public async Task<ActionResult<ResponseModel<object>>> GetAllBySellProductId(string sellProductId)
        {
            var response = new ResponseModel<object>();
            try
            {
                var comments = await commentService.GetAlCommentlBySellProductIdAsync(sellProductId);
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
                var ratings = await commentService.GetAllRatingBySellProductIdAsync(sellProductId);
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

        [Authorize(Roles = "user")]
        [HttpPost("create-comment")]
        public async Task<ActionResult<ResponseModel<object>>> CreateComment([FromBody] CommentCreateDto dto)
        {
            var response = new ResponseModel<object>();
            try
            {
                var (account, _, _, _) = await authService.GetUserWithTokens(HttpContext);

                var comment = await commentService.CreateCommentAsync(dto.SellProductId, account.Id, dto.Content);
                response.Success = true;
                response.Data = comment;
                return Ok(response);
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ResponseModel<object>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 403
                });
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorCode = 400;
                return BadRequest(response);
            }
        }

        [Authorize(Roles = "user")]
        [HttpPost("create-rating-only")]
        public async Task<ActionResult<ResponseModel<object>>> CreateRatingOnly([FromBody] CommentRatingCreateDto dto)
        {
            var response = new ResponseModel<object>();
            try
            {
                var (account, _, _, _) = await authService.GetUserWithTokens(HttpContext);

                var comment = await commentService.CreateRatingOnlyAsync(dto.SellProductId, account.Id, dto.Rating);
                response.Success = true;
                response.Data = comment;
                return Ok(response);
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ResponseModel<object>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 403
                });
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorCode = 400;
                return BadRequest(response);
            }
        }

        [HttpGet("get-all-comment-of-sellproduct")]
        public async Task<ActionResult<ResponseModel<object>>> GetAllCommentProductOfUser(string userId,string productName)
        {
            var response = new ResponseModel<object>();
            try
            {
                var comments = await commentService.GetAllCommentProductOfUserAsync(userId, productName);
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

        [HttpGet("get-rating-of-user")]
        public async Task<ActionResult<ResponseModel<object>>> GetRatingOfUser(string userId)
        {
            var response = new ResponseModel<object>();
            try
            {
                var comments = await commentService.GetRatingOfUser(userId);
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
        [HttpGet("get-rating-average-of-sell-product")]
        public async Task<ActionResult<ResponseModel<object>>> GetRatingAverageOfSellproduct(string sellProductId)
        {
            var response = new ResponseModel<object>();
            try
            {
                var comments = await commentService.GetTotalAverageOfSellProductByIdAsync(sellProductId);
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
    }
}