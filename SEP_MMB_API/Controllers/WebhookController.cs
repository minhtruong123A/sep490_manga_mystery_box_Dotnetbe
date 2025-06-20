using BusinessObjects.Dtos.PayOS;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using System.Text;


namespace SEP_MMB_API.Controllers
{
    [ApiController]
    [Route("api/webhook")]
    public class WebhookController : ControllerBase
    {
        private readonly IConfiguration _config;

        public WebhookController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("payment")]
        public async Task<IActionResult> HandlePaymentWebhook([FromBody] PayOSWebhookRequest request)
        {
            // 1. Kiểm tra signature
            var checksumKey = _config["PayOS:ChecksumKey"];
            var rawData = Newtonsoft.Json.JsonConvert.SerializeObject(request.Data); // serialize data
            var computedSignature = ComputeHmacSHA256(rawData, checksumKey);

            if (computedSignature != request.Signature)
            {
                return BadRequest("Invalid signature");
            }

            // 2. Xử lý nếu thanh toán thành công
            if (request.Success && request.Data.Code == "00")
            {
                var orderCode = request.Data.OrderCode;
                var amount = request.Data.Amount;

                // TODO: xử lý logic cộng tiền, lưu lịch sử
                // await yourService.ProcessRechargeAsync(orderCode, amount);

                return Ok(new { success = true });
            }

            return BadRequest("Payment failed or code invalid");
        }

        private static string ComputeHmacSHA256(string data, string key)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
