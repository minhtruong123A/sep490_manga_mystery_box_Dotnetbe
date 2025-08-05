using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.PayOS
{
    public class PayOSWebhookRequest
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("desc")]
        public string Desc { get; set; }

        [JsonPropertyName("data")]
        public PayOSWebhookData Data { get; set; }

        [JsonPropertyName("signature")]
        public string Signature { get; set; }
    }
}
