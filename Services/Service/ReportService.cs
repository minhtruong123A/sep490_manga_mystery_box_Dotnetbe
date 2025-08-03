using BusinessObjects;
using BusinessObjects.Dtos.Report;
using DataAccessLayers.Interface;
using DataAccessLayers.UnitOfWork;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _uniUnitOfWork;
        private readonly IOrderHistoryService _orderHistoryService;
        private readonly IProductService _productService;

        public ReportService(IUnitOfWork uniUnitOfWork, IOrderHistoryService orderHistoryService, IProductService productService)
        {
            _uniUnitOfWork = uniUnitOfWork;
            _orderHistoryService = orderHistoryService;
            _productService = productService;
        }

        public async Task<bool> CreateReportAsync(ReportCreateDto dto, string userId) => await _uniUnitOfWork.ReportRepository.CreateReportAsync(dto, userId);

        public async Task<List<Report>> GetAllAsync()
        {
           var result = await _uniUnitOfWork.ReportRepository.GetAllAsync();
           return result.ToList();
        }

        public async Task<List<ReportResponeDto>> GetAllReportOfUserAsync(string userId) => await _uniUnitOfWork.ReportRepository.GetAllReportOfUserAsync(userId);

        public async Task<SalesReportDto> GetSalesReportAsync(string userId)
        {
            var allOrderHistory = await _orderHistoryService.GetOrderHistoryAsync(userId);
            var productSellOrders = allOrderHistory.Where(x => x.Type == "ProductSell").ToList();
            var totalRevenue = productSellOrders.Sum(o => o.TotalAmount);
            var totalOrders = productSellOrders.Count;
            var totalProductsSold = productSellOrders.Sum(o => o.Quantity);
            var byDay = productSellOrders
                .GroupBy(o => o.PurchasedAt.Date)
                .Select(g => new TimelineRevenueDto
                {
                    Time = g.Key.ToString("dd/MM/yyyy"),
                    Revenue = g.Sum(o => o.TotalAmount),
                    Orders = g.Count(),
                    ProductsSold = g.Sum(o => o.Quantity)
                })
                .OrderBy(x => x.Time)
                .ToList();

            var byMonth = productSellOrders
                .GroupBy(o => new { o.PurchasedAt.Year, o.PurchasedAt.Month })
                .Select(g => new TimelineRevenueDto
                {
                    Time = $"{g.Key.Month:00}/{g.Key.Year}",
                    Revenue = g.Sum(o => o.TotalAmount),
                    Orders = g.Count(),
                    ProductsSold = g.Sum(o => o.Quantity)
                })
                .OrderBy(x => x.Time)
                .ToList();

            var byYear = productSellOrders
                .GroupBy(o => o.PurchasedAt.Year)
                .Select(g => new TimelineRevenueDto
                {
                    Time = g.Key.ToString(),
                    Revenue = g.Sum(o => o.TotalAmount),
                    Orders = g.Count(),
                    ProductsSold = g.Sum(o => o.Quantity)
                })
                .OrderBy(x => x.Time)
                .ToList();

            var topProductGroups = productSellOrders
                .GroupBy(o => new { o.SellProductId, o.ProductId, o.ProductName })
                .Select(g => new
                {
                    g.Key.SellProductId,
                    g.Key.ProductId,
                    g.Key.ProductName,
                    TotalSold = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.TotalAmount),
                    TransactionCodes = g.Select(x => x.TransactionCode).Distinct().ToList()
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(5)
                .ToList();

            var topProducts = new List<TopProductDto>();

            foreach (var item in topProductGroups)
            {
                var productInfo = await _productService.GetProductWithRarityByIdAsync(item.ProductId);

                topProducts.Add(new TopProductDto
                {
                    SellProductId = item.SellProductId ?? "Unknown",
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    TotalSold = item.TotalSold,
                    TotalRevenue = item.TotalRevenue,
                    UrlImage = productInfo?.UrlImage ?? string.Empty,
                    TransactionCodes = item.TransactionCodes
                });
            }

            return new SalesReportDto
            {
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                TotalProductsSold = totalProductsSold,
                ByDay = byDay,
                ByMonth = byMonth,
                ByYear = byYear,
                TopProducts = topProducts
            };
        }
    }
}
