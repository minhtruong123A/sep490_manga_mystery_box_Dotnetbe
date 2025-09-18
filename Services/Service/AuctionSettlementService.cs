using BusinessObjects;
using BusinessObjects.Dtos.Auction;
using BusinessObjects.Options;
using DataAccessLayers.Interface;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Services.Interface;

namespace Services.Service;

public class AuctionSettlementService : IAuctionSettlementService
{
    private readonly FeeSettings _feeSettings;
    private readonly IMongoClient _mongoClient;
    private readonly IUnitOfWork _unitOfWork;

    public AuctionSettlementService(IUnitOfWork unitOfWork, IMongoClient mongoClient, IOptions<FeeSettings> feeOptions)
    {
        _unitOfWork = unitOfWork;
        _mongoClient = mongoClient;
        _feeSettings = feeOptions.Value;
    }

    public async Task<bool> FinalizeAuctionResultAsync(string auctionId)
    {
        using (var session = await _mongoClient.StartSessionAsync())
        {
            session.StartTransaction();
            try
            {
                var auctionResult =
                    await _unitOfWork.AuctionResultRepository.FindOneAsync(session, x => x.AuctionId == auctionId) ??
                    throw new Exception("AuctionResult not found");
                if (auctionResult.IsSolved) throw new Exception("AuctionResult has already been finalized");

                var bidderUser =
                    await _unitOfWork.UserRepository.FindOneAsync(session, x => x.Id == auctionResult.BidderId) ??
                    throw new Exception("Bidder user not found");
                var bidderWallet =
                    await _unitOfWork.UseDigitalWalletRepository.FindOneAsync(session,
                        x => x.Id == bidderUser.WalletId) ?? throw new Exception("Bidder Wallet not found");
                if (bidderWallet.Ammount < auctionResult.BidderAmount) throw new Exception("Insufficient balance");

                bidderWallet.Ammount -= auctionResult.BidderAmount;
                await _unitOfWork.UseDigitalWalletRepository.UpdateAsync(session, bidderWallet.Id, bidderWallet);

                var bidderPayment = new AuctionPaymentSession
                {
                    AuctionResultId = auctionResult.Id,
                    WalletId = bidderWallet.Id,
                    UserId = auctionResult.BidderId,
                    Amount = (int)auctionResult.BidderAmount,
                    IsWithdraw = false,
                    Type = "Bidder"
                };
                await _unitOfWork.AuctionPaymentSessionRepository.AddAsync(session, bidderPayment);

                var hosterUser =
                    await _unitOfWork.UserRepository.FindOneAsync(session, x => x.Id == auctionResult.HosterId) ??
                    throw new Exception("Hoster user not found");
                var hosterWallet =
                    await _unitOfWork.UseDigitalWalletRepository.FindOneAsync(session,
                        x => x.Id == hosterUser.WalletId) ?? throw new Exception("Hoster Wallet not found");
                hosterWallet.Ammount += auctionResult.HostClaimAmount;
                await _unitOfWork.UseDigitalWalletRepository.UpdateAsync(session, hosterWallet.Id, hosterWallet);

                var hosterPayment = new AuctionPaymentSession
                {
                    AuctionResultId = auctionResult.Id,
                    WalletId = hosterWallet.Id,
                    UserId = auctionResult.HosterId,
                    Amount = (int)auctionResult.HostClaimAmount,
                    IsWithdraw = true,
                    Type = "Hoster"
                };
                await _unitOfWork.AuctionPaymentSessionRepository.AddAsync(session, hosterPayment);

                var feeRate = 0.05m;
                var feeAmount = (int)(auctionResult.BidderAmount * feeRate);
                var transactionFee = new TransactionFee
                {
                    ReferenceId = auctionResult.Id,
                    ReferenceType = "Auction",
                    FromUserId = auctionResult.HosterId,
                    ProductId = auctionResult.ProductId,
                    GrossAmount = (int)auctionResult.BidderAmount,
                    FeeAmount = feeAmount,
                    FeeRate = _feeSettings.AuctionFeeRate,
                    Type = "AuctionResult",
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.TransactionFeeRepository.AddAsync(session, transactionFee);
                await UpdateUserCollectionAndProductAsync(session, auctionResult.BidderId, auctionResult.ProductId,
                    auctionResult.Quantity);

                auctionResult.IsSolved = true;
                await _unitOfWork.AuctionResultRepository.UpdateAsync(session, auctionResult.Id, auctionResult);

                await session.CommitTransactionAsync();

                /* await _unitOfWork.UserAchievementRepository.CheckAchievement(auctionResult.BidderId);*/
                return true;
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();
                throw new Exception($"FinalizeAuction failed. Reason: {ex.Message}");
            }
        }
    }

    public async Task<bool> ChangeStatusAsync(string auctionSessionId, int status)
    {
        var auctionSession = await _unitOfWork.AuctionSessionRepository.GetByIdAsync(auctionSessionId);
        if (auctionSession == null) throw new Exception("AuctionSession not founded");
        if (auctionSession.Status == 0)
        {
            auctionSession.Status = status;
            await _unitOfWork.AuctionSessionRepository.UpdateAsync(auctionSessionId, auctionSession);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<AuctionResultDto?> GetAuctionResultByIdAsync(string auctionResultId)
    {
        var auctionResult = await _unitOfWork.AuctionResultRepository.FindOneAsync(ar => ar.Id == auctionResultId);
        if (auctionResult == null) return null;

        var product = await _unitOfWork.ProductRepository.FindOneAsync(p => p.Id == auctionResult.ProductId);
        if (product == null) throw new Exception("Product not found");

        var rarity = await _unitOfWork.RarityRepository.FindOneAsync(r => r.Id == product.RarityId);
        if (rarity == null) throw new Exception("Rarity not found");

        var bidder = await _unitOfWork.UserRepository.FindOneAsync(u => u.Id == auctionResult.BidderId);
        var hoster = await _unitOfWork.UserRepository.FindOneAsync(u => u.Id == auctionResult.HosterId);

        return new AuctionResultDto
        {
            Id = auctionResult.Id,
            AuctionId = auctionResult.AuctionId,
            ProductId = product.Id,
            ProductName = product.Name,
            ProductDescription = product.Description,
            RarityId = rarity.Id,
            RarityName = rarity.Name,
            BidderId = auctionResult.BidderId,
            BidderUsername = bidder?.Username,
            BidderProfileImage = bidder?.ProfileImage,
            HosterId = auctionResult.HosterId,
            HosterUsername = hoster?.Username,
            HosterProfileImage = hoster?.ProfileImage,
            Quantity = auctionResult.Quantity,
            BidderAmount = auctionResult.BidderAmount,
            HostClaimAmount = auctionResult.HostClaimAmount,
            IsSolved = auctionResult.IsSolved
        };
    }

    private async Task UpdateUserCollectionAndProductAsync(IClientSessionHandle session, string bidderId,
        string productId, int quantity)
    {
        var productInBox =
            await _unitOfWork.ProductInMangaBoxRepository.FindOneAsync(session, p => p.ProductId == productId) ??
            throw new Exception("ProductInMangaBox not found");
        var mangaBox = await _unitOfWork.MangaBoxRepository.GetByIdAsync(session, productInBox.MangaBoxId) ??
                       throw new Exception("MangaBox not found");
        var collectionId = mangaBox.CollectionTopicId;
        var userCollection = await _unitOfWork.UserCollectionRepository.FindOneAsync(session,
            c => c.UserId == bidderId && c.CollectionId == collectionId);
        if (userCollection == null)
        {
            userCollection = new UserCollection
            {
                UserId = bidderId,
                CollectionId = collectionId
            };
            await _unitOfWork.UserCollectionRepository.AddAsync(session, userCollection);
        }

        var userProduct = await _unitOfWork.UserProductRepository.FindOneAsync(session,
            up => up.CollectorId == bidderId && up.ProductId == productId);
        if (userProduct != null)
        {
            var update = Builders<UserProduct>.Update.Inc(x => x.Quantity, quantity);
            var updateIsQuantityUpdateInc = Builders<UserProduct>.Update.Set(x => x.isQuantityUpdateInc, true);
            var updateUpdateAt = Builders<UserProduct>.Update.Set(x => x.UpdateAt, DateTime.UtcNow);
            await _unitOfWork.UserProductRepository.UpdateFieldAsync(session, x => x.Id == userProduct.Id, update);
            await _unitOfWork.UserProductRepository.UpdateFieldAsync(session, x => x.Id == userProduct.Id,
                updateIsQuantityUpdateInc);
            await _unitOfWork.UserProductRepository.UpdateFieldAsync(session, x => x.Id == userProduct.Id,
                updateUpdateAt);
            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            var newUserProduct = new UserProduct
            {
                ProductId = productId,
                CollectorId = bidderId,
                Quantity = quantity,
                CollectedAt = DateTime.UtcNow,
                CollectionId = userCollection.Id,
                isQuantityUpdateInc = true,
                UpdateAt = DateTime.UtcNow
            };
            await _unitOfWork.UserProductRepository.AddAsync(session, newUserProduct);
        }
    }
}