using System.Text.Json.Serialization;

namespace BusinessObjects.Dtos.User;

public class UserTokenDto
{
    [JsonPropertyName("access_token")] public string? AccessToken { get; set; }

    //[JsonPropertyName("refresh_token")]
    //public string? RefreshToken { get; set; }
    [JsonPropertyName("token_type")] public string? TokenType { get; set; }

    [JsonPropertyName("user_id")] public string? UserId { get; set; }

    [JsonPropertyName("username")] public string? UserName { get; set; }

    [JsonPropertyName("wallet_amount")] public decimal? Amount { get; set; }

    [JsonPropertyName("profile_image")] public string? ProfileImage { get; set; }

    [JsonPropertyName("role")] public string? RoleId { get; set; }
}