using BusinessObjects.Dtos.TransactionFee;
using DataAccessLayers.Interface;
using Services.Interface;

namespace Services.Service;

public class TransactionFeeService(IUnitOfWork unitOfWork) : ITransactionFeeService
{
    public async Task<List<TransactionFeeDto>> GetAllValidTransactionFeesAsync()
    {
        var transactionFees = await unitOfWork.TransactionFeeRepository.GetAllAsync();
        var orderHistories = await unitOfWork.OrderHistoryRepository.GetAllAsync();
        var auctionResults = await unitOfWork.AuctionResultRepository.GetAllAsync();

        var validReferenceIds = new HashSet<string>(
            orderHistories.Select(o => o.Id)
                .Concat(auctionResults.Select(a => a.Id))
        );

        var users = await unitOfWork.UserRepository.GetAllAsync();
        var products = await unitOfWork.ProductRepository.GetAllAsync();

        var validTransactionFees = transactionFees
            .Where(tf => validReferenceIds.Contains(tf.ReferenceId))
            .Select(tf =>
            {
                var user = users.FirstOrDefault(u => u.Id == tf.FromUserId);
                var product = products.FirstOrDefault(p => p.Id == tf.ProductId);

                return new TransactionFeeDto
                {
                    Id = tf.Id,
                    ReferenceId = tf.ReferenceId,
                    ReferenceType = tf.ReferenceType,
                    Type = tf.Type,
                    GrossAmount = tf.GrossAmount,
                    FeeAmount = tf.FeeAmount,
                    FeeRate = tf.FeeRate,
                    CreatedAt = tf.CreatedAt,

                    FromUserId = tf.FromUserId,
                    Username = user?.Username,
                    ProfileImage = user?.ProfileImage,

                    ProductId = tf.ProductId,
                    ProductName = product?.Name,
                    RarityId = product?.RarityId,
                    UrlImage = product?.UrlImage,
                    Is_Block = product?.Is_Block ?? false
                };
            })
            .ToList();

        return validTransactionFees;
    }
}