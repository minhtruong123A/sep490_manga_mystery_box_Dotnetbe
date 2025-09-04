using BusinessObjects.Dtos.Report;

namespace Services.Interface;

public interface IReportService
{
    Task<bool> CreateReportAsync(ReportCreateDto dto, string userId);
    Task<List<ReportResponeDto>> GetAllReportAsync();
    Task<List<ReportResponeDto>> GetAllReportOfUserAsync(string userId);
    Task<SalesReportDto> GetSalesReportAsync(string userId);
    Task<bool> UpdateStatus(string reportId);
}