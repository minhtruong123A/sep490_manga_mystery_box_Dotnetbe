using BusinessObjects;
using BusinessObjects.Dtos.OrderHistory;
using BusinessObjects.Enum;
using BusinessObjects.Options;
using DataAccessLayers.Interface;
using Microsoft.Extensions.Options;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class OrderHistoryService : IOrderHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly FeeSettings _feeSettings;

        public OrderHistoryService(IUnitOfWork unitOfWork, IOptions<FeeSettings> feeOptions)
        {
            _unitOfWork = unitOfWork;
            _feeSettings = feeOptions.Value;
        }

        public async Task<List<OrderHistoryDto>> GetOrderHistoryAsync(string userId)
        {
            var user = await _unitOfWork.UserRepository.FindOneAsync(u => u.Id == userId) ?? throw new Exception("User not found");
            var result = new List<OrderHistoryDto>();
            var boxOrders = await _unitOfWork.BoxOrderRepository.FilterByAsync(b => b.UserId == userId);
            if (boxOrders?.Any() == true)
            {
                var boxOrderIds = boxOrders.Select(b => b.Id.ToString()).ToList();
                var orderHistories = await _unitOfWork.OrderHistoryRepository.FilterByAsync(h => boxOrderIds.Contains(h.BoxOrderId));
                var orderHistoryIds = orderHistories.Select(h => h.Id.ToString()).ToList();

                var mangaBoxes = await _unitOfWork.MangaBoxRepository.GetAllAsync();
                var mangaBoxDict = mangaBoxes.ToDictionary(x => x.Id, x => x);

                var mysteryBoxes = await _unitOfWork.MysteryBoxRepository.GetAllAsync();
                var mysteryBoxDict = mysteryBoxes.ToDictionary(x => x.Id, x => x);

                var paymentSessions = await _unitOfWork.DigitalPaymentSessionRepository.FilterByAsync(p => orderHistoryIds.Contains(p.OrderId) && p.Type == nameof(DigitalPaymentSessionType.MysteryBox));
                var paymentDict = paymentSessions.ToDictionary(p => p.OrderId, p => p);

                foreach (var orderHis in orderHistories)
                {
                    var boxOrder = boxOrders.FirstOrDefault(b => b.Id.ToString() == orderHis.BoxOrderId);

                    if (boxOrder == null) continue;
                    if (!mangaBoxDict.TryGetValue(boxOrder.BoxId, out var mangaBox)) continue;
                    if (!mysteryBoxDict.TryGetValue(mangaBox.MysteryBoxId, out var mysteryBox)) continue;
                    if (!paymentDict.TryGetValue(orderHis.Id.ToString(), out var payment)) continue;

                    result.Add(new OrderHistoryDto
                    {
                        Type = "Box",
                        BoxId = mangaBox.Id,
                        SellProductId = null,
                        SellerUsername = null,
                        SellerUrlImage = null,
                        IsSellSellProduct = null,
                        BoxName = mysteryBox.Name,
                        Quantity = boxOrder.Quantity,
                        TotalAmount = boxOrder.Amount,
                        TransactionCode = payment.Id.ToString(),
                        PurchasedAt = orderHis.Datetime
                    });
                }
            }

            var productOrders = await _unitOfWork.productOrderRepository.FilterByAsync(p => p.BuyerId == userId || p.SellerId == userId);
            if (productOrders?.Any() == true)
            {
                var productOrderIds = productOrders.Select(p => p.Id.ToString()).ToList();
                var productOrderHistories = await _unitOfWork.OrderHistoryRepository.FilterByAsync(h => productOrderIds.Contains(h.ProductOrderId));

                var historyIds = productOrderHistories.Select(h => h.Id.ToString()).ToList();
                var paymentSessions = await _unitOfWork.DigitalPaymentSessionRepository.FilterByAsync(p => historyIds.Contains(p.OrderId) && p.Type == nameof(DigitalPaymentSessionType.SellProduct));
                var paymentDict = paymentSessions.ToDictionary(p => p.OrderId, p => p);

                var sellProductIds = productOrders.Select(p => p.SellProductId).Distinct().ToList();
                var sellProducts = await _unitOfWork.SellProductRepository.FilterByAsync(sp => sellProductIds.Contains(sp.Id));
                var sellProductDict = sellProducts.ToDictionary(sp => sp.Id, sp => sp);
                var productIds = sellProducts.Select(sp => sp.ProductId).Distinct().ToList();
                var products = await _unitOfWork.ProductRepository.FilterByAsync(p => productIds.Contains(p.Id));
                var productDict = products.ToDictionary(p => p.Id, p => p);

                var sellerIds = productOrders.Select(p => p.SellerId).Distinct().ToList();
                var sellers = await _unitOfWork.UserRepository.FilterByAsync(u => sellerIds.Contains(u.Id));
                var sellerDict = sellers.ToDictionary(u => u.Id, u => u);

                var addedTransactionCodes = new HashSet<string>();
                var tempResult = new List<OrderHistoryDto>();

                foreach (var order in productOrders)
                {
                    if (!sellProductDict.TryGetValue(order.SellProductId, out var sellProduct)) continue;
                    if (!productDict.TryGetValue(sellProduct.ProductId, out var product)) continue;

                    sellerDict.TryGetValue(order.SellerId, out var seller);

                    var relatedHistories = productOrderHistories.Where(h => h.ProductOrderId == order.Id.ToString()).ToList();
                    if (!relatedHistories.Any()) continue;

                    foreach (var history in relatedHistories)
                    {
                        if (!paymentDict.TryGetValue(history.Id.ToString(), out var payment)) continue;
                        if (order.SellerId == userId)
                        {
                            if (addedTransactionCodes.Contains(payment.Id.ToString())) continue;

                            addedTransactionCodes.Add(payment.Id.ToString());
                            tempResult.Add(new OrderHistoryDto
                            {
                                Type = "ProductSell",
                                SellProductId = order.SellProductId,
                                ProductId = product.Id,
                                ProductName = product.Name,
                                SellerUsername = seller?.Username,
                                SellerUrlImage = seller?.ProfileImage,
                                IsSellSellProduct = sellProduct?.IsSell,
                                Quantity = 1,
                                TotalAmount = (int)Math.Floor(payment.Amount * (1-_feeSettings.AuctionFeeRate)),
                                TransactionCode = payment.Id.ToString(),
                                PurchasedAt = history.Datetime,
                                transactionFeeRate = _feeSettings.AuctionFeeRate
                            });
                        }
                        else if (order.BuyerId == userId)
                        {
                            if (addedTransactionCodes.Contains(payment.Id.ToString())) continue;

                            addedTransactionCodes.Add(payment.Id.ToString());
                            tempResult.Add(new OrderHistoryDto
                            {
                                Type = "ProductBuy",
                                SellProductId = order.SellProductId,
                                ProductId = product.Id,
                                ProductName = product.Name,
                                SellerUsername = seller?.Username,
                                SellerUrlImage = seller?.ProfileImage,
                                IsSellSellProduct = sellProduct?.IsSell,
                                Quantity = 1,
                                TotalAmount = payment.Amount,
                                TransactionCode = payment.Id.ToString(),
                                PurchasedAt = history.Datetime
                            });
                        }
                    }
                }

                var mergedResult = tempResult
                    .Where(x => x.Quantity == 1)
                    .GroupBy(x => new
                    {
                        x.ProductId,
                        Minute = x.PurchasedAt.Year + "-" + x.PurchasedAt.Month + "-" + x.PurchasedAt.Day + " " + x.PurchasedAt.Hour + ":" + x.PurchasedAt.Minute
                    })
                    .SelectMany(g =>
                    {
                        if (g.Count() == 1)
                            return (IEnumerable<OrderHistoryDto>)g;

                        var matched = g.FirstOrDefault(x =>
                            (x.Type == "ProductSell" && userId == productOrders.FirstOrDefault(o => o.SellProductId == x.SellProductId)?.SellerId)
                            ||
                            (x.Type == "ProductBuy" && userId == productOrders.FirstOrDefault(o => o.SellProductId == x.SellProductId)?.BuyerId)
                        );

                        return matched != null ? new List<OrderHistoryDto> { matched } : new List<OrderHistoryDto>();
                    })
                    .ToList();

                var leftOvers = tempResult.Where(x => x.Quantity != 1).ToList();
                result.AddRange(mergedResult.Concat(leftOvers).OrderByDescending(x => x.PurchasedAt));
            }

            return result.OrderByDescending(r => r.PurchasedAt).ToList();
        }

        public async Task<List<UserOrderHistoryResultDto>> GetAllUserOrderHistoriesAsync()
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();

            var tasks = users.Select(async user =>
            {
                var history = await GetOrderHistoryAsync(user.Id);
                return new UserOrderHistoryResultDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    OrderHistories = history
                };
            });

            return (await Task.WhenAll(tasks)).ToList();
        }

        public async Task<OrderHistoryDto?> GetOrderHistoryByIdAsync(string orderHistoryId)
        {
            var orderHistory = await _unitOfWork.OrderHistoryRepository.FindOneAsync(h => h.Id == orderHistoryId);
            if (orderHistory == null) return null;

            if (!string.IsNullOrEmpty(orderHistory.BoxOrderId))
            {
                if (!string.IsNullOrEmpty(orderHistory.BoxOrderId))
                {
                    var boxOrder = await _unitOfWork.BoxOrderRepository.FindOneAsync(b => b.Id.ToString() == orderHistory.BoxOrderId);
                    if (boxOrder == null) return null;

                    var mangaBox = await _unitOfWork.MangaBoxRepository.FindOneAsync(m => m.Id == boxOrder.BoxId);
                    if (mangaBox == null) return null;

                    var mysteryBox = await _unitOfWork.MysteryBoxRepository.FindOneAsync(m => m.Id == mangaBox.MysteryBoxId);
                    if (mysteryBox == null) return null;

                    var payment = await _unitOfWork.DigitalPaymentSessionRepository.FindOneAsync(p => p.OrderId == orderHistory.Id.ToString() && p.Type == nameof(DigitalPaymentSessionType.MysteryBox));
                    if (payment == null) return null;

                    return new OrderHistoryDto
                    {
                        Type = "Box",
                        BoxId = mangaBox.Id,
                        BoxName = mysteryBox.Name,
                        Quantity = boxOrder.Quantity,
                        TotalAmount = boxOrder.Amount,
                        TransactionCode = payment.Id.ToString(),
                        PurchasedAt = orderHistory.Datetime
                    };
                }
            }

            if (!string.IsNullOrEmpty(orderHistory.ProductOrderId))
            {
                if (!string.IsNullOrEmpty(orderHistory.ProductOrderId))
                {
                    var productOrder = await _unitOfWork.productOrderRepository.FindOneAsync(p => p.Id.ToString() == orderHistory.ProductOrderId);
                    if (productOrder == null) return null;

                    var sellProduct = await _unitOfWork.SellProductRepository.FindOneAsync(sp => sp.Id == productOrder.SellProductId);
                    if (sellProduct == null) return null;

                    var product = await _unitOfWork.ProductRepository.FindOneAsync(p => p.Id == sellProduct.ProductId);
                    if (product == null) return null;

                    var seller = await _unitOfWork.UserRepository.FindOneAsync(u => u.Id == productOrder.SellerId);

                    var payment = await _unitOfWork.DigitalPaymentSessionRepository.FindOneAsync(p => p.OrderId == orderHistory.Id.ToString() && p.Type == nameof(DigitalPaymentSessionType.SellProduct));
                    if (payment == null) return null;

                    var isBuyer = productOrder.BuyerId == seller?.Id;

                    return new OrderHistoryDto
                    {
                        Type = isBuyer ? "ProductBuy" : "ProductSell",
                        SellProductId = productOrder.SellProductId,
                        ProductId = product.Id,
                        ProductName = product.Name,
                        SellerUsername = seller?.Username,
                        SellerUrlImage = seller?.ProfileImage,
                        IsSellSellProduct = sellProduct?.IsSell,
                        Quantity = 1,
                        TotalAmount = isBuyer ? payment.Amount : (int)Math.Floor(payment.Amount * (1 - _feeSettings.AuctionFeeRate)),
                        TransactionCode = payment.Id.ToString(),
                        PurchasedAt = orderHistory.Datetime,
                        transactionFeeRate = _feeSettings.AuctionFeeRate
                    };
                }
            }

            return null;
        }
    }
}