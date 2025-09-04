using System.Text.Json.Serialization;

namespace BusinessObjects.Dtos.Schema_Response;

public class ResponseModel<T> where T : class
{
    public ResponseModel()
    {
        Success = true;
    }

    [JsonPropertyName("status")] public bool Success { get; set; }

    [JsonPropertyName("data")] public T? Data { get; set; }

    [JsonPropertyName("error")] public string? Error { get; set; }

    [JsonPropertyName("errorCode")] public int ErrorCode { get; set; }
}