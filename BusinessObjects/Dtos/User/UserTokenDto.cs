using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.User
{
    public class UserTokenDto
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
        //[JsonPropertyName("refresh_token")]
        //public string? RefreshToken { get; set; }
        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }
        [JsonPropertyName("username")]
        public string? UserName { get; set; }
        [JsonPropertyName("role")]
        public string? RoleId { get; set; }
    }
}
