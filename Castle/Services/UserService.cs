using Castle.Api.Auth;
using Castle.Dao;
using Castle.Helpers;
using Castle.Interfaces;
using Castle.Models.User;
using Microsoft.Extensions.Options;
using BCryptNet = BCrypt.Net.BCrypt;

namespace Castle.Services
{
    public class UserService : IUserService
    {
        private IJwtUtils _jwtUtils;
        private readonly AppSettings _appSettings;
        private readonly IDatabaseService _databaseService;

        public UserService(IJwtUtils jwtUtils, IOptions<AppSettings> appSettings, IDatabaseService databaseService)
        {
            _jwtUtils = jwtUtils;
            _appSettings = appSettings.Value;
            _databaseService = databaseService;
        }


        public async Task<AuthResponse?> Register(RegisterRequest model)
        {
            var conn = _databaseService.GetAsyncConnection();
            var checkDuplicate = await conn.Table<UserDao>().FirstOrDefaultAsync(u => u.Username == model.Username);

            if (checkDuplicate != null)
            {
                //throw new AppException("Username already exists");
                Console.WriteLine($"Duplicate User? >> Id: {((User)checkDuplicate).Id}");
                return null;
            }

            var user = new User
            {
                Username = model.Username,
                Password = BCryptNet.HashPassword(model.Password),
                Role = Role.User
            };
            await conn.InsertAsync((UserDao)user);

            var u = await conn.Table<UserDao>().FirstOrDefaultAsync(u => u.Username == model.Username);  // THIS IS A DUMB WAY OF DOING THIS ISNT IT ???
            user.Id = u.Id;

            var jwtToken = _jwtUtils.GenerateJwtToken(user);

            return new AuthResponse(user, jwtToken); // Id will always be zero because the auto-increment happens in the db
        }

        public async Task<AuthResponse?> RegisterAdmin(RegisterAdminRequest model)
        {

            var conn = _databaseService.GetAsyncConnection();
            var checkDuplicate = await conn.Table<UserDao>().FirstOrDefaultAsync(u => u.Username == model.Username);

            if (checkDuplicate != null)
            {
                //throw new AppException("Username already exists");
                Console.WriteLine($"Duplicate User? >> Id: {((User)checkDuplicate).Id}");
                return null;
            }

            if (model.AdminPassword != "AdminPassword") // Hardcoded
            {
                //throw new AppException("Registration Failed");
                return null;
            }

            var user = new User
            {
                Username = model.Username,
                Password = BCryptNet.HashPassword(model.Password),
                Role = Role.Admin
            };
            await conn.InsertAsync((UserDao)user);

            var u = await conn.Table<UserDao>().FirstOrDefaultAsync(u => u.Username == model.Username);  // THIS IS A DUMB WAY OF DOING THIS ISNT IT ???
            user.Id = u.Id;

            var jwtToken = _jwtUtils.GenerateJwtToken(user);

            return new AuthResponse(user, jwtToken); // Id will always be zero because the auto-increment happens in the db
        }

        public async Task<AuthResponse?> Authenticate(AuthRequest model)
        {
            var conn = _databaseService.GetAsyncConnection();

            var user = await conn.Table<UserDao>().FirstOrDefaultAsync(u => u.Username == model.Username);

            // Validate
            if (user == null || !BCryptNet.Verify(model.Password, user.Password))
            {
                //throw new AppException("Username or password is incorrect");
                return null;
            }

            // gen JWT
            var jwtToken = _jwtUtils.GenerateJwtToken(user);

            return new AuthResponse(user, jwtToken);
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            var conn = _databaseService.GetAsyncConnection();
            var users = await conn.Table<UserDao>().ToArrayAsync();

            return users.Select(u => (User)u);
        }

        //public async Task<User?> GetById(int id) // HAVING THIS ASYNC CAUSES AN ERROR FOR CASTING IN MY AuthorizeAttribute implementation
        //{
        //    var conn = _databaseService.GetAsyncConnection();
        //    var user = await conn.Table<UserDao>().FirstOrDefaultAsync(u => u.Id == id);


        //    return user != null ? (User)user : null;
        //}

        public User? GetById(int id)
        {
            var conn = _databaseService.GetConnection();
            //var user =  conn.Table<UserDao>().FirstOrDefault(u => u.Id == id);
            var user = conn.Table<UserDao>().FirstOrDefault(u => u.Id == id);

            return user != null ? (User)user : null;
        }
    }
}
