using System.Security.Cryptography;
using System.Text;

namespace Week5Project.Helpers
{
    public static class AuthHelper
    {
        public static string GenerateToken(string username)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(username + DateTime.Now.Ticks));
            return Convert.ToBase64String(bytes);
        }
    }
}