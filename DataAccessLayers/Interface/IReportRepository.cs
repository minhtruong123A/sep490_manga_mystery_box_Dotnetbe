using BusinessObjects;
using BusinessObjects.Dtos.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Interface
{
    public interface IReportRepository : IGenericRepository<Report>
    {
        Task<bool> CreateReportAsync(ReportCreateDto dto, string userId);
        Task<List<ReportResponeDto>> GetAllReportOfUserAsync(string userId);
    }
}
