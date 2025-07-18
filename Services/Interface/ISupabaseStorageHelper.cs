using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ISupabaseStorageHelper
    {
        Task<string> CreateSignedUrlAsync(string path, int? expiresIn = null);
        Task<string> UploadImageAsync(IFormFile file);
        Task DeleteImageAsync(string fileName);
    }
}
