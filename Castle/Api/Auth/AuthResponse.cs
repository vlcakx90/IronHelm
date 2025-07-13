using Castle.Models.User;

namespace Castle.Api.Auth
{
    public class AuthResponse
    {
        public int Id { get; set; }
        public string? Username { get; set; }

        public Role Role { get; set; }
        public string Token { get; set; }


        public AuthResponse(User user, string token)
        {
            Id = user.Id;
            Username = user.Username;
            Role = user.Role;
            Token = token;
        }
    }
}
