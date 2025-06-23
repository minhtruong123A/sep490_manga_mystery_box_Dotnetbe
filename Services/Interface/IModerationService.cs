using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IModerationService
    {
        //Task<bool> IsContentSafeOpenAIAsync(string content);
        Task<bool> IsContentSafeGeminiAIAsync(string content);
    }
}
