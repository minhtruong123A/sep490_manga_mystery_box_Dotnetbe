using BusinessObjects.Dtos.PayOS;
using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayOSController : ControllerBase
    {
        private readonly IPayOSService _payOSService;

        public PayOSController(IPayOSService payOSService)
        {
            _payOSService = payOSService;
        }

        [HttpPost("create-payment")]
        public async Task<ActionResult<ResponseModel<object>>> CreatePayment([FromBody] CreatePaymentRequest req)
        {
            try
            {
                long orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
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

                var result = await _payOSService.CreatePaymentLinkAsync(orderCode, totalAmount, "Recharge", items);

                return Ok(new ResponseModel<object>
                {
                    Data = new
                    {
                        checkoutUrl = result.checkoutUrl,
                        qrCode = result.qrCode,
                        orderCode = result.orderCode
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

        [HttpGet("test-create")]
        public async Task<IActionResult> TestCreate()
        {
            long orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var items = new List<ItemData> { new ItemData("Gói test VIP", 1, 20000) };
            var result = await _payOSService.CreatePaymentLinkAsync(
                orderCode,
                amount: 20000,
                description: "Test giao diện thanh toán",
                items: items
            );

            return Redirect(result.checkoutUrl);
        }
    }
}
