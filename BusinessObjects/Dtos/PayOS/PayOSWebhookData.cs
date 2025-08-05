using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BusinessObjects.Dtos.PayOS
{
    public class PayOSWebhookData
    {
        [JsonProperty("orderCode")]
        public long OrderCode { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("transactionDateTime")]
        public string TransactionDateTime { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("paymentLinkId")]
        public string PaymentLinkId { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("desc")]
        public string Desc { get; set; }

        [JsonProperty("counterAccountBankId")]
        public string CounterAccountBankId { get; set; }

        [JsonProperty("counterAccountBankName")]
        public string CounterAccountBankName { get; set; }

        [JsonProperty("counterAccountName")]
        public string CounterAccountName { get; set; }

        [JsonProperty("counterAccountNumber")]
        public string CounterAccountNumber { get; set; }

        [JsonProperty("virtualAccountName")]
        public string VirtualAccountName { get; set; }

        [JsonProperty("virtualAccountNumber")]
        public string VirtualAccountNumber { get; set; }
    }
}
