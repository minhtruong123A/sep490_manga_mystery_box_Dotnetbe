using BusinessObjects;
using BusinessObjects.Dtos.Feedback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IFeedbackService
    {
        Task<bool> CreateFeedbackAsync(string userId, FeedbackCreateDto dto);
        Task<List<FeedbackResponeDto>> GetAllFeedbackOfProductSaleAsync(string sellProductId);
    }
}
