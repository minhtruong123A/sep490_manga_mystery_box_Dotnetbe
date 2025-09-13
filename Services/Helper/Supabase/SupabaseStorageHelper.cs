using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Services.Interface;

namespace Services.Helper.Supabase;

public class SupabaseStorageHelper : ISupabaseStorageHelper
{
    private readonly HttpClient _httpClient;
    private readonly SupabaseSettings _settings;

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

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0) throw new Exception("File is empty.");

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var uploadUrl = $"{_settings.Url}/storage/v1/object/{_settings.Bucket}/{fileName}";

        using var content = new StreamContent(file.OpenReadStream());
        content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        var response = await _httpClient.PostAsync(uploadUrl, content);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Upload failed: {error}");
        }

        return fileName;
    }

    public async Task DeleteImageAsync(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return;

        var deleteUrl = $"{_settings.Url}/storage/v1/object/{_settings.Bucket}/{fileName}";

        var request = new HttpRequestMessage(HttpMethod.Delete, deleteUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ServiceRoleKey);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Delete failed: {error}");
        }
    }

    public async Task<string> UploadSystemProductImageAsync(IFormFile file, string fileNameToUse)
    {
        if (file == null || file.Length == 0) throw new Exception("File is empty.");

        //var fileName = Path.GetFileName(file.FileName);
        var exists = await FileExistsAsync(fileNameToUse);
        if (exists) throw new Exception("A file with the same name already exists.");

        var uploadUrl = $"{_settings.Url}/storage/v1/object/{_settings.Bucket}/{fileNameToUse}";

        using var content = new StreamContent(file.OpenReadStream());
        content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        var request = new HttpRequestMessage(HttpMethod.Post, uploadUrl)
        {
            Content = content
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ServiceRoleKey);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Upload failed: {error}");
        }

        return fileNameToUse;
    }

    public async Task<bool> FileExistsAsync(string fileName)
    {
        var checkUrl = $"{_settings.Url}/storage/v1/object/info/{_settings.Bucket}/{fileName}";

        var request = new HttpRequestMessage(HttpMethod.Get, checkUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ServiceRoleKey);

        var response = await _httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<string>> ListFilesByPrefixAsync(string prefix)
    {
        var listUrl = $"{_settings.Url}/storage/v1/object/list/{_settings.Bucket}";
        var payload = new { prefix };
        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, listUrl)
        {
            Content = content
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ServiceRoleKey);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to list files from Supabase: {error}");
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        var fileObjects = JsonSerializer.Deserialize<List<SupabaseFileObject>>(responseBody);

        return fileObjects?.Select(f => f.Name).ToList() ?? new List<string>();
    }
}