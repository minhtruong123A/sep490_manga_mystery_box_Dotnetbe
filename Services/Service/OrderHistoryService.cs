using BusinessObjects.Dtos.OrderHistory;
using BusinessObjects.Enum;
using DataAccessLayers.Interface;
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

        public OrderHistoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<OrderHistoryDto>> GetOrderHistoryAsync(string userId)
        {
            var user = await _unitOfWork.UserRepository.FindOneAsync(u => u.Id == userId) ?? throw new Exception("User not found");
            var result = new List<OrderHistoryDto>();

            var boxOrders = await _unitOfWork.BoxOrderRepository.FilterByAsync(b => b.UserId == userId);
            if (boxOrders != null && boxOrders.Any())
            {
                var mangaBoxes = await _unitOfWork.MangaBoxRepository.GetAllAsync();
                var mysteryBoxes = await _unitOfWork.MysteryBoxRepository.GetAllAsync();
                var mangaBoxDict = mangaBoxes.ToDictionary(x => x.Id, x => x);
                var mysteryBoxDict = mysteryBoxes.ToDictionary(x => x.Id, x => x);
                var orderHistories = await _unitOfWork.OrderHistoryRepository.FilterByAsync(h => boxOrders.Select(b => b.Id.ToString()).Contains(h.BoxOrderId));
                var orderHistoryIds = orderHistories.Select(h => h.Id.ToString()).ToList();
                var paymentSessions = await _unitOfWork.DigitalPaymentSessionRepository.FilterByAsync(p => orderHistoryIds.Contains(p.OrderId)
                                                                                                            && p.Type == nameof(DigitalPaymentSessionType.MysteryBox));
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
                        BoxName = mysteryBox.Name,
                        Quantity = boxOrder.Quantity,
                        TotalAmount = boxOrder.Amount,
                        TransactionCode = payment.Id.ToString(),
                        PurchasedAt = orderHis.Datetime
                    });
                }
            }

            var productOrders = await _unitOfWork.productOrderRepository.FilterByAsync(p => p.BuyerId == userId);
            if (productOrders != null && productOrders.Any())
            {
                var productOrderIds = productOrders.Select(p => p.Id.ToString()).ToList();
                var productOrderHistories = await _unitOfWork.OrderHistoryRepository.FilterByAsync(h => productOrderIds.Contains(h.ProductOrderId));
                var productOrderHistoryIds = productOrderHistories.Select(h => h.Id.ToString()).ToList();
                var paymentProductSessions = await _unitOfWork.DigitalPaymentSessionRepository.FilterByAsync(p => productOrderHistoryIds.Contains(p.OrderId) && p.Type == nameof(DigitalPaymentSessionType.SellProduct));
                var paymentProductDict = paymentProductSessions.ToDictionary(p => p.OrderId, p => p);
                var productIds = productOrders.Select(p => p.ProductId).ToList();
                var products = await _unitOfWork.ProductRepository.FilterByAsync(p => productIds.Contains(p.Id));
                var productDict = products.ToDictionary(p => p.Id, p => p);

                foreach (var orderHis in productOrderHistories)
                {
                    var productOrder = productOrders.FirstOrDefault(p => p.Id.ToString() == orderHis.ProductOrderId);
                    if (productOrder == null) continue;
                    if (!productDict.TryGetValue(productOrder.ProductId, out var product)) continue;
                    if (!paymentProductDict.TryGetValue(orderHis.Id.ToString(), out var payment)) continue;

                    result.Add(new OrderHistoryDto
                    {
                        Type = "Product",
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        TotalAmount = productOrder.Amount,
                        TransactionCode = payment.Id.ToString(),
                        PurchasedAt = orderHis.Datetime
                    });
                }
            }

            return result.OrderByDescending(r => r.PurchasedAt).ToList();
        }

    }
}