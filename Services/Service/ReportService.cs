using BusinessObjects;
using BusinessObjects.Dtos.Report;
using DataAccessLayers.Interface;
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
        public ReportService(IUnitOfWork uniUnitOfWork) 
        { 
        _uniUnitOfWork = uniUnitOfWork;
        }

        public async Task<bool> CreateReportAsync(ReportCreateDto dto, string userId) => await _uniUnitOfWork.ReportRepository.CreateReportAsync(dto, userId);

        public async Task<List<Report>> GetAllAsync()
        {
           var result = await _uniUnitOfWork.ReportRepository.GetAllAsync();
           return result.ToList();
        }

        public async Task<List<Report>> GetAllReportOfUser(string userId) => await _uniUnitOfWork.ReportRepository.GetAllReportOfUserAsync(userId);
    }
}
