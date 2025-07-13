using Castle.Helpers;
using Castle.Interfaces;
using Castle.Models.User;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Castle.Auth
{
    public class JwtUtils : IJwtUtils
    {
        private readonly AppSettings _appSettings;

        public JwtUtils(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;

            if (string.IsNullOrEmpty(_appSettings.Secret))
            {
                //throw new Exception("JWT secret not configured");

                Console.WriteLine("\n\n\n!!! Fix This !!!");
                Console.WriteLine("[!] JWT secret not configured");
                Console.WriteLine("[*] Setting JWT Secret in JwtUtils Constr\n\n\n");
                _appSettings.Secret = "58455132E50AC3AFB8A70F8763BD0C9DF78F2EA5740EB9F4C9E30C2F4CB967D95BA3AF7657B77FA4DB82F8D814ADA12E86C790E3F78C798758DBFE81936A7B2E06B295ACE82FD554C4EF411E535D7F4B841570E5129A6DA7CF1271CBA75FAB74F079DDE8897FC01994587B";
            }
        }

        public string GenerateJwtToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.GivenName, user.Username)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public int? ValidateJwtToken(string? token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret!);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                // return user id from JWT token if validation successful
                return userId;
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }

        private JwtSecurityToken? DecodeToken(string tokenString)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(tokenString);
            var token = jsonToken as JwtSecurityToken;


            return token;
        }

        public bool IsTokenExpired(string tokenString)
        {
            var decodedToken = DecodeToken(tokenString);
            if (decodedToken == null)
            {
                throw new NullReferenceException("[!] Token Decode Failed");
            }

            var expiration = decodedToken.ValidTo;
            var now = DateTime.Now;
            var diff = DateTime.Compare(now, expiration);

            if (diff <= 0)
            {
                // valid (not expired): not at expiration date yet
                return false;
            }
            else
            {
                // invalid (expired)
                return true;
            }
        }

        public DateTime? GetTokenExpirationDate(string tokenString)
        {
            var decodedToken = DecodeToken(tokenString);
            if (decodedToken == null)
            {
                //throw new NullReferenceException("[!] Token Decode Failed");
                return null;
            }

            var expiration = decodedToken.ValidTo;

            return expiration;
        }

        public string? GetTokenUserName(string tokenString)
        {
            var decodedToken = DecodeToken(tokenString);
            if (decodedToken == null)
            {
                //throw new NullReferenceException("[!] Token Decode Failed");
                return null;
            }

            var givenName = decodedToken.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.GivenName);
            if (givenName == null)
            {
                return null;
            }

            var username = givenName.Value;

            return username;
        }
    }
}
