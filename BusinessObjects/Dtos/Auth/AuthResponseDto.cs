using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Auth
{
    public class AuthResponseDto
    {
        public string? Token { get; set; }
        public string RefreshToken { get; set; }
        public string TokenType { get; set; } = "bearer";
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsEmailVerification { get; set; }
    }
}
