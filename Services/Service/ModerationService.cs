using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;
using Services.Interface;

namespace Services.Service;

public class ModerationService : IModerationService
{
    private readonly string _apiGeminiKey;
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    public ModerationService(IConfiguration configuration)
    {
        _httpClient = new HttpClient();
        _apiKey = configuration["OpenAI:ApiKey"]?.Trim();
        _apiGeminiKey = configuration["Gemini:ApiKey"]?.Trim();
    }

    //public async Task<bool> IsContentSafeOpenAIAsync(string content)
    //{
    //    try
    //    {
    //        var data = new { input = content };
    //        using var requestJson = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
    //        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/moderations")
    //        { Content = requestJson };
    //        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    //        Console.WriteLine("DATA: " + JsonSerializer.Serialize(data));
    //        using var response = await _httpClient.SendAsync(request);
    //        Console.WriteLine("Request JSON: " + await requestJson.ReadAsStringAsync());
    //        var jsonResponse = await response.Content.ReadAsStringAsync();

    //        if (!response.IsSuccessStatusCode) throw new HttpRequestException($"OpenAI API error ({response.StatusCode}): {jsonResponse}");

    //        using var document = JsonDocument.Parse(jsonResponse);
    //        bool flagged = document.RootElement.GetProperty("results")[0].GetProperty("flagged").GetBoolean();

    //        return !flagged;
    //    }
    //    catch (HttpRequestException ex)
    //    {
    //        throw new Exception("Failed to send content to OpenAI Moderation API.", ex);
    //    }
    //    catch (JsonException ex)
    //    {
    //        throw new Exception("Failed to parse OpenAI Moderation API response.", ex);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new Exception("An unexpected error occurred in moderation service.", ex);
    //    }
    //}

    public async Task<bool> IsContentSafeGeminiAIAsync(string content)
    {
        try
        {
            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text =
                                    $"Is the following message toxic, offensive, or inappropriate? Answer 'True' if yes, 'False' if safe. Message: \"{content}\""
                            }
                        }
                    }
                }
            };
            if (string.IsNullOrWhiteSpace(_apiGeminiKey))
                throw new InvalidOperationException("Gemini API key is not configured.");

            var request = new HttpRequestMessage(HttpMethod.Post,
                    $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={_apiGeminiKey}")
                { Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json") };
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await JsonSerializer.DeserializeAsync<JsonNode>(await response.Content.ReadAsStreamAsync());
            var answer = body?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

            return answer?.Trim().Equals("false", StringComparison.OrdinalIgnoreCase) == true;
        }
        catch (Exception ex)
        {
            throw new Exception("Gemini AI moderation failed.", ex);
        }
    }
}