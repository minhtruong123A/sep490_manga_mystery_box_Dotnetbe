using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class SignedUrlService : ISignedUrlService
    {
        private readonly ISupabaseStorageHelper _supabaseStorageHelper;

        public SignedUrlService(ISupabaseStorageHelper supabaseStorageHelper)
        {
            _supabaseStorageHelper = supabaseStorageHelper;
        }

        public async Task<string?> GetSignedUrlIfAvailableAsync(string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;

            try
            {
                var signedUrl = await _supabaseStorageHelper.CreateSignedUrlAsync(path);
                return signedUrl;
            }
            catch { return null; }
        }
    }
}
