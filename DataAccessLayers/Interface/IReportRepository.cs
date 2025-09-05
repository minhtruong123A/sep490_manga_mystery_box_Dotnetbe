using BusinessObjects;
using BusinessObjects.Dtos.Report;

namespace DataAccessLayers.Interface;

public interface IReportRepository : IGenericRepository<Report>
{
    Task<bool> CreateReportAsync(ReportCreateDto dto, string userId);
    Task<List<ReportResponeDto>> GetAllReportAsync();
    Task<List<ReportResponeDto>> GetAllReportOfUserAsync(string userId);
}