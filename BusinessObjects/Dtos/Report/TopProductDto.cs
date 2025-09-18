namespace BusinessObjects.Dtos.Report;

public class TopProductDto
{
    public string SellProductId { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }

    public string UrlImage { get; set; }

    public int TotalSold { get; set; }
    public int TotalRevenue { get; set; }

    public List<string> TransactionCodes { get; set; }
}