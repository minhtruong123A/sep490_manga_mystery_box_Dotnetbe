using BusinessObjects;
using BusinessObjects.Dtos.Feedback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Interface
{
    public interface IFeedbackRepository : IGenericRepository<Feedback>
    {
        Task<List<FeedbackResponeDto>> GetAllFeedbackOfProductSaleAsync(string sellProductId);
        Task<bool> CheckExistFeedbackAsync(string userId);
    }
}
