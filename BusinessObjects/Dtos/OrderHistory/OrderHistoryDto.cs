namespace BusinessObjects.Dtos.OrderHistory;

public class OrderHistoryDto
{
    public string Type { get; set; }

    public string BoxId { get; set; }
    public string BoxName { get; set; }

    public string? SellProductId { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public string? SellerUsername { get; set; }
    public string? SellerUrlImage { get; set; }
    public bool? IsSellSellProduct { get; set; }
    public double? transactionFeeRate { get; set; }


    public int Quantity { get; set; }
    public int TotalAmount { get; set; }
    public string TransactionCode { get; set; }
    public DateTime PurchasedAt { get; set; }
}