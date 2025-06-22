using BusinessObjects.Dtos.PayOS;
using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using Services.Helper;
using Services.Interface;
using System.Text;


namespace SEP_MMB_API.Controllers
{
    [ApiController]
    [Route("api/webhook")]
    public class WebhookController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IPayOSService _payOSService;

        public WebhookController(IConfiguration config, IPayOSService payOSService)
        {
            _config = config;
            _payOSService = payOSService;
        }

        [HttpPost("payment")]
        public async Task<ActionResult<ResponseModel<object>>> HandlePaymentWebhook([FromBody] PayOSWebhookRequest request)
        {
            try
            {
                var checksumKey = _config["PayOS:ChecksumKey"];
                var rawData = Newtonsoft.Json.JsonConvert.SerializeObject(request.Data);
                var computedSignature = HmacHelper.ComputeHmacSHA256(rawData, checksumKey);

                if (computedSignature != request.Signature)
                {
                    return BadRequest(new ResponseModel<object>
                    {
                        Data = null,
                        Success = false,
                        Error = "Invalid signature",
                        ErrorCode = 400
                    });
                }

                if (request.Success && request.Data.Code == "00")
                {
                    var orderCode = request.Data.OrderCode.ToString();
                    var amount = request.Data.Amount;
                    var success = await _payOSService.ProcessRechargeAsync(orderCode, amount);

                    if (!success)
                    {
                        return BadRequest(new ResponseModel<object>
                        {
                            Data = null,
                            Success = false,
                            Error = "Recharge failed.",
                            ErrorCode = 400
                        });
                    }

                    return Ok(new ResponseModel<object>
                    {
                        Data = new { message = "Recharge successful" },
                        Success = true,
                        Error = null
                    });
                }

                return BadRequest(new ResponseModel<object>
                {
                    Data = null,
                    Success = false,
                    Error = "Payment failed or system invalid",
                    ErrorCode = 400
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
