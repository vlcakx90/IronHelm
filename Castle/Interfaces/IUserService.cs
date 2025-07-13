using Castle.Api.Auth;
using Castle.Models.User;

namespace Castle.Interfaces
{
    public interface IUserService
    {
        Task<AuthResponse?> Authenticate(AuthRequest model);

        Task<AuthResponse?> Register(RegisterRequest model);
        Task<AuthResponse?> RegisterAdmin(RegisterAdminRequest model);
        Task<IEnumerable<User>> GetAll();
        //Task<User?> GetById(int id);
        User? GetById(int id);
    }
}
