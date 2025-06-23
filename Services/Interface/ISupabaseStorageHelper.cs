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
    }
}
