using BusinessObjects;
using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IReportService
    {
        Task<bool> CreateReportAsync(ReportCreateDto dto, string userId);
        Task<List<Report>> GetAllAsync();
        Task<List<ReportResponeDto>> GetAllReportOfUserAsync(string userId);
        Task<SalesReportDto> GetSalesReportAsync(string userId);
        Task<bool> UpdateStatus(string reportId);
    }
}
