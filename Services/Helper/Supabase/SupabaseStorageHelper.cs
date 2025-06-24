using Microsoft.Extensions.Options;
using Services.Interface;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Services.Helper.Supabase
{
    public class SupabaseStorageHelper : ISupabaseStorageHelper
    {
        private readonly SupabaseSettings _settings;
        private readonly HttpClient _httpClient;

        public SupabaseStorageHelper(IOptions<SupabaseSettings> options)
        {
            _settings = options.Value;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("apikey", _settings.ServiceRoleKey);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ServiceRoleKey}");
        }

        public async Task<string> CreateSignedUrlAsync(string path, int? expiresIn = null)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new Exception("Path is empty");

            var expireSeconds = expiresIn ?? _settings.SignedUrlExpireSeconds;
            var requestUrl = $"{_settings.Url}/storage/v1/object/sign/{_settings.Bucket}/{path}";

            Console.WriteLine($"Signing Supabase URL: {requestUrl}");

            var body = new { expiresIn = expireSeconds };
            var requestContent = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(requestUrl, requestContent);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to get signed URL: {error}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var signedResponse = JsonSerializer.Deserialize<SupabaseSignedUrlResponse>(responseContent);
            if (signedResponse?.signedURL == null) throw new Exception("Signed URL not found in response");

            var fullUrl = $"{_settings.Url}/storage/v1/{signedResponse.signedURL}";

            return fullUrl;
        }
    }
}
