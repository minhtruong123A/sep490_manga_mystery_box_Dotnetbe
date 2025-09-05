using Services.Interface;

namespace Services.Service;

public class SignedUrlService(ISupabaseStorageHelper supabaseStorageHelper) : ISignedUrlService
{
    public async Task<string?> GetSignedUrlIfAvailableAsync(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return null;

        try
        {
            var signedUrl = await supabaseStorageHelper.CreateSignedUrlAsync(path);
            return signedUrl;
        }
        catch
        {
            return null;
        }
    }
}