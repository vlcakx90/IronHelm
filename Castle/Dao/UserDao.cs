using Castle.Models.User;
using SQLite;

namespace Castle.Dao
{
    [Table("users")]
    public class UserDao
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        public int Id { get; set; }


        [Column("username")]
        public string Username { get; set; } = string.Empty;

        [Column("role")]
        public Role Role { get; set; }

        [Column("password")]
        public string Password { get; set; } = string.Empty;


        public static implicit operator UserDao(User user)
        {
            return new UserDao
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                Password = user.Password
            };
        }

        public static implicit operator User(UserDao dao)
        {
            return new User
            {
                Id = dao.Id,
                Username = dao.Username,
                Role = dao.Role,
                Password = dao.Password
            };
        }
    }
}
