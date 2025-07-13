using Castle.Models.User;
using System.IdentityModel.Tokens.Jwt;

namespace Castle.Interfaces
{
    public interface IJwtUtils
    {
        public string GenerateJwtToken(User user);
        public int? ValidateJwtToken(string? token);
        public bool IsTokenExpired(string tokenString);
        public DateTime? GetTokenExpirationDate(string tokenString);
        public string? GetTokenUserName(string tokenString);

    }
}
