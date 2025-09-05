using BusinessObjects.Dtos.Auction;

namespace Services.Interface;

public interface IAuctionSettlementService
{
    Task<bool> FinalizeAuctionResultAsync(string auctionId);
    Task<bool> ChangeStatusAsync(string auctionSessionId, int status);
    Task<AuctionResultDto?> GetAuctionResultByIdAsync(string auctionResultId);
}