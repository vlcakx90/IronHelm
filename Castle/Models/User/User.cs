using System.Text.Json.Serialization;

namespace Castle.Models.User
{
    public class User
    {
        public int Id { get; set; }
        public string? Username { get; set; }

        public Role Role { get; set; }

        [JsonIgnore] // To prevent the password from being returned in API HTTP responses
        public string? Password { get; set; }
    }
}
