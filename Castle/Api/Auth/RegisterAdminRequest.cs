using System.ComponentModel.DataAnnotations;

namespace Castle.Api.Auth
{
    public class RegisterAdminRequest
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        public string? AdminPassword { get; set; }
    }
}
