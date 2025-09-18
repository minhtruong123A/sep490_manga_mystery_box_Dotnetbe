using System.Security.Cryptography;
using System.Text;

namespace Services.Helper;

public static class HmacHelper
{
    public static string ComputeHmacSHA256(string data, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}