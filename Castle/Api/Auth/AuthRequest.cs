using System.ComponentModel.DataAnnotations;

namespace Castle.Api.Auth
{
    public class AuthRequest
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
