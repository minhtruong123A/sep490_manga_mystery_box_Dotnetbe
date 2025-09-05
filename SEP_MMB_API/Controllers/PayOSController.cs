using BusinessObjects.Dtos.PayOS;
using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayOSController(IPayOSService payOsService, IAuthService authService) : ControllerBase
    {
        [Authorize(Roles = "user")]
        [HttpPost("create-payment")]
        public async Task<ActionResult<ResponseModel<object>>> CreatePayment([FromBody] CreatePaymentRequest req)
        {
            try
            {
                var (account, _, _, _) = await authService.GetUserWithTokens(HttpContext);
                var items = req.Items.Select(x => new ItemData(x.Name, 1, x.Price)).ToList();
                int totalAmount = items.Sum(i => i.price * i.quantity);
                if (totalAmount <= 0)
                {
                    return BadRequest(new ResponseModel<object>
                    {
                        Data = null,
                        Success = false,
                        Error = "Total prices must be greater than 0.",
                        ErrorCode = 400
                    });
                }

                long orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var result = await payOsService.CreatePaymentLinkAsync(
                    orderCode,
                    totalAmount,
                    "Recharge",
                    items,
                    account.Id
                );

                return Ok(new ResponseModel<object>
                {
                    Data = new
                    {
                        checkoutUrl = result.checkoutUrl,
                        qrCode = result.qrCode,
                        orderCode = result.orderCode,
                        userid = account.Id,
                        totalAmount = totalAmount,
                    },
                    Success = true,
                    Error = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<object>
                {
                    Data = null,
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }

        [HttpPost("check-transactions")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> CheckAndUpdateTransactions()
        {
            try
            {
                var result = await payOsService.CheckAndUpdatePendingTransactionsAsync();
                return Ok(new
                {
                    message = "Checked and updated.",
                    updated = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(400, new
                {
                    message = "Error occurred.",
                    error = ex.Message
                });
            }
        }

        [Authorize(Roles = "user")]
        [HttpGet("payment-status")]
        public async Task<ActionResult<ResponseModel<object>>> GetPaymentStatus([FromQuery] string orderCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(orderCode))
                {
                    return BadRequest(new ResponseModel<object>
                    {
                        Data = null,
                        Success = false,
                        Error = "orderCode is required",
                        ErrorCode = 400
                    });
                }

                var status = await payOsService.GetPayOSStatusViaSdkAsync(orderCode);

                return Ok(new ResponseModel<object>
                {
                    Data = new
                    {
                        orderCode,
                        status
                    },
                    Success = true,
                    Error = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<object>
                {
                    Data = null,
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }
    }
}
