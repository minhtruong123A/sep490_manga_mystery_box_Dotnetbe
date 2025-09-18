using System.Text.Json;
using BusinessObjects.Dtos.PayOS;
using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Mvc;
using Services.Helper;
using Services.Interface;

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

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("payment")]
        public async Task<IActionResult> HandlePaymentWebhook()
        {
            try
            {
                _logger.LogInformation("Webhook entered");
                _logger.LogInformation("Content-Type: {ContentType}", Request.ContentType);
                Request.EnableBuffering();
                var rawBody = await new StreamReader(Request.Body).ReadToEndAsync();
                _logger.LogInformation("Raw request body: {Body}", rawBody);
                Request.Body.Position = 0;
                using var doc = JsonDocument.Parse(rawBody);
                if (!doc.RootElement.TryGetProperty("data", out var dataElement))
                {
                    _logger.LogWarning("Missing 'data' property in request");
                    return BadRequest(new ResponseModel<object>
                    {
                        Data = null,
                        Success = false,
                        Error = "Missing data field",
                        ErrorCode = 400
                    });
                }

                var dataDict = JsonSerializer.Deserialize<Dictionary<string, object>>(dataElement.GetRawText());
                var sortedData = dataDict
                    .OrderBy(kvp => kvp.Key, StringComparer.Ordinal)
                    .Select(kvp => $"{kvp.Key}={(kvp.Value?.ToString() ?? "")}");
                var signatureBase = string.Join("&", sortedData);
                var checksumKey = _config["PayOS:ChecksumKey"];
                var computedSignature = HmacHelper.ComputeHmacSHA256(signatureBase, checksumKey);
                var signature = doc.RootElement.GetProperty("signature").GetString();
                //var signature = doc.RootElement.GetProperty("signature").GetString();
                //var checksumKey = _config["PayOS:ChecksumKey"];
                //var computedSignature = HmacHelper.ComputeHmacSHA256(rawData, checksumKey);
                //_logger.LogInformation("Raw request.Data: {data}", rawData);
                //_logger.LogInformation("Incoming signature: {signature}", signature);
                //_logger.LogInformation("Computed signature: {computed}", computedSignature);
                _logger.LogInformation("Signature base string: {Base}", signatureBase);
                _logger.LogInformation("Incoming signature: {signature}", signature);
                _logger.LogInformation("Computed signature: {computed}", computedSignature);

                if (!computedSignature.Equals(signature, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Invalid signature");
                    return BadRequest(new ResponseModel<object>
                    {
                        Data = null,
                        Success = false,
                        Error = "Invalid signature",
                        ErrorCode = 400
                    });
                }

                var request = JsonSerializer.Deserialize<PayOSWebhookRequest>(rawBody);
                if (request?.Code == "00" && request?.Data?.Code == "00")
                {
                    //var orderCode = request.Data.OrderCode;
                    var amount = request.Data.Amount;
                    var orderCode = request.Data.OrderCode.ToString();
                    if (await _payOSService.HasOrderBeenProcessedAsync(orderCode))
                    {
                        _logger.LogInformation("Order {OrderCode} already processed", orderCode);
                        return Ok(new ResponseModel<object>
                        {
                            Data = new { message = "Order already processed" },
                            Success = true
                        });
                    }

                    var success = await _payOSService.ProcessRechargeAsync(orderCode, amount);
                    _logger.LogInformation("Recharge result for order {OrderCode}: {Result}", orderCode, success);

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
