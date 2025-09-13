namespace BusinessObjects.Dtos.Exchange;

public class ExchangeGetAllWithProductDto
{
    public string Id { get; set; }
    public string BuyerId { get; set; }
    public string BuyerName { get; set; }
    public string BuyerImage { get; set; }
    public string SellerId { get; set; }
    public string SellerName { get; set; }
    public string SellerImage { get; set; }
    public string ItemReciveId { get; set; }
    public string IamgeItemRecive { get; set; }
    public string ItemGiveId { get; set; }
    public int Status { get; set; }
    public DateTime Datetime { get; set; }
    public DateTime Enddate { get; set; }
    public bool IsFeedback { get; set; }
    public List<ExchangeProductDetailDto> Products { get; set; }
}