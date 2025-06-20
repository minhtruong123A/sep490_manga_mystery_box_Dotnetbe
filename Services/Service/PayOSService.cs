using BusinessObjects.Dtos.PayOS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class PayOSService : IPayOSService
    {
        private readonly PayOS _payOS;

        public PayOSService(IOptions<PayOSConfig> config)
        {
            var cfg = config.Value;
            _payOS = new PayOS(cfg.ClientId, cfg.ApiKey, cfg.ChecksumKey);
        }

        public async Task<CreatePaymentResult> CreatePaymentLinkAsync(
            long orderCode,
            int amount,
            string description,
            List<ItemData> items
        )
        {
            var paymentData = new PaymentData(
                orderCode,
                amount,
                description,
                items,
                cancelUrl: "https://yourfrontend.com/cancel", // trang dang ky nap
                returnUrl: "https://yourfrontend.com/success" // trang dang ky nap
            );

            return await _payOS.createPaymentLink(paymentData);
        }
    }
}
