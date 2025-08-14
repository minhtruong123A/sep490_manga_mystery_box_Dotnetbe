using BusinessObjects.Dtos.OrderHistory;
using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderHistoryController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IOrderHistoryService _orderHistoryService;

        public OrderHistoryController(IAuthService authService, IOrderHistoryService orderHistoryService)
        {
            _authService = authService;
            _orderHistoryService = orderHistoryService;
        }

        [Authorize(Roles = "user")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<List<OrderHistoryDto>>>> GetOrderHistory()
        {
            try
            {
                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
                var result = await _orderHistoryService.GetOrderHistoryAsync(account.Id);

                return Ok(new ResponseModel<List<OrderHistoryDto>>
                {
                    Success = true,
                    Data = result,
                    Error = null,
                    ErrorCode = 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<List<OrderHistoryDto>>
                {
                    Success = false,
                    Data = null,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseModel<List<UserOrderHistoryResultDto>>>> GetAllUserOrderHistories()
        {
            try
            {
                var result = await _orderHistoryService.GetAllUserOrderHistoriesAsync();

                return Ok(new ResponseModel<List<UserOrderHistoryResultDto>>
                {
                    Success = true,
                    Data = result,
                    Error = null,
                    ErrorCode = 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<List<UserOrderHistoryResultDto>>
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
