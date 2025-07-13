using System.ComponentModel.DataAnnotations;

namespace Castle.Api.Auth
{
    public class RegisterRequest
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
