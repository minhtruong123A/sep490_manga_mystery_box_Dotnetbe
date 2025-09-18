namespace BusinessObjects.Dtos.Auction;

public class AuctionResultDto
{
    public string Id { get; set; }
    public string AuctionId { get; set; }

    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductDescription { get; set; }
    public string RarityId { get; set; }
    public string RarityName { get; set; }

    public string BidderId { get; set; }
    public string BidderUsername { get; set; }
    public string BidderProfileImage { get; set; }

    public string HosterId { get; set; }
    public string HosterUsername { get; set; }
    public string HosterProfileImage { get; set; }

    public int Quantity { get; set; }
    public decimal BidderAmount { get; set; }
    public decimal HostClaimAmount { get; set; }
    public bool IsSolved { get; set; }
}