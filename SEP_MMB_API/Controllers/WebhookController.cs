using BusinessObjects.Dtos.PayOS;
using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using Services.Helper;
using Services.Interface;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace SEP_MMB_API.Controllers
{
    [ApiController]
    [Route("api/webhook")]
    public class WebhookController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IPayOSService _payOSService;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(IConfiguration config, IPayOSService payOSService, ILogger<WebhookController> logger)
        {
            _config = config;
            _payOSService = payOSService;
            _logger = logger;
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("payment")]
        public async Task<IActionResult> HandlePaymentWebhook([FromBody] JObject body)
        {
            try
            {
                _logger.LogInformation("Attempting to parse webhook...");

                var request = body.ToObject<PayOSWebhookRequest>();
                _logger.LogInformation("Deserialized request: {request}", JsonConvert.SerializeObject(request));

                if (request == null)
                {
                    _logger.LogWarning("request is null");
                    return BadRequest(new ResponseModel<object>
                    {
                        Data = null,
                        Success = false,
                        Error = "Request is null",
                        ErrorCode = 400
                    });
                }

                if (request.Data == null)
                {
                    _logger.LogWarning("request.Data is null. Raw input: {raw}", body.ToString());
                    return BadRequest(new ResponseModel<object>
                    {
                        Data = null,
                        Success = false,
                        Error = "Missing data field",
                        ErrorCode = 400
                    });
                }

                var checksumKey = _config["PayOS:ChecksumKey"];
                var rawData = JsonConvert.SerializeObject(request.Data);
                var computedSignature = HmacHelper.ComputeHmacSHA256(rawData, checksumKey);
                _logger.LogInformation("Parsed request: {request}", JsonConvert.SerializeObject(request));
                _logger.LogInformation("Parsed request.Data: {data}", JsonConvert.SerializeObject(request?.Data));
                _logger.LogInformation("Incoming signature: {signature}", request?.Signature);
                _logger.LogInformation("Computed signature: {computed}", computedSignature);
                if (!computedSignature.Equals(request.Signature, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Invalid signature for order {OrderCode}", request.Data.OrderCode);
                    return BadRequest(new ResponseModel<object>
                    {
                        Data = null,
                        Success = false,
                        Error = "Invalid signature",
                        ErrorCode = 400
                    });
                }

                if (request.Code == "00" && request.Data?.Code == "00")
                {
                    var orderCode = request.Data.OrderCode.ToString();
                    var amount = request.Data.Amount;
                    if (await _payOSService.HasOrderBeenProcessedAsync(orderCode))
                    {
                        return Ok(new ResponseModel<object>
                        {
                            Data = new { message = "Order already processed" },
                            Success = true
                        });
                    }

                    var success = await _payOSService.ProcessRechargeAsync(orderCode, amount);

                    return Ok(new ResponseModel<object>
                    {
                        Data = new { message = success ? "Recharge successful" : "Recharge failed" },
                        Success = success
                    });
                }

                return Ok(new ResponseModel<object>
                {
                    Data = null,
                    Success = false,
                    Error = "Payment failed or invalid system response"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while handling PayOS webhook");
                return Ok(new ResponseModel<object>
                {
                    Data = null,
                    Success = false,
                    Error = "Internal error: " + ex.Message
                });
            }
        }

    }
}
