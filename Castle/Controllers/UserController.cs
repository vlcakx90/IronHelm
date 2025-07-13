using Castle.Api.Auth;
using Castle.Auth;
using Castle.Interfaces;
using Castle.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace Castle.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRequestHeader _requestHeader;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, IRequestHeader requestHeader, ILogger<UserController> logger)
        {
            _userService = userService;
            _requestHeader = requestHeader;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _userService.Register(model);

            if (response == null)
            {
#if DEBUG
                _logger.LogError($"[!] Register() > registration failed for: " +
                    $"{model.Username}" +
                    $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

                return BadRequest(new { message = "Registration failed" });
            }

#if DEBUG
            _logger.LogInformation($"[+] Register() > registering for: " +
                $"{model.Username}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("registeradmin")]
        public async Task<IActionResult> RegisterAdmin(RegisterAdminRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _userService.RegisterAdmin(model);

            if (response == null)
            {
#if DEBUG
                _logger.LogError($"[!] RegisterAdmin() > registration failed for: " +
                    $"{model.Username}" +
                    $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

                return BadRequest(new { message = "Registration failed" });
            }

#if DEBUG
            _logger.LogInformation($"[+] RegisterAdmin() > registering for: " +
                $"{model.Username}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(AuthRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _userService.Authenticate(model);

            if (response == null)
            {
#if DEBUG
                _logger.LogError($"[!] Authenticat() > authentication failed for: " +
                    $"{model.Username}" +
                    $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

                return BadRequest(new { message = "Username or password is incorrect" });
            }

#if DEBUG
            _logger.LogInformation($"[+] Authenticat() > authentication for: " +
                $"{model.Username}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

            return Ok(response);
        }

        [Authorize(Role.Admin)]
        [HttpGet(Name = "Users")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAll();

#if DEBUG
            _logger.LogInformation($"[+] GetAll() > user: " +
                $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

            return Ok(users);
        }

        [HttpGet("{id:int}", Name = "User")]
        public IActionResult GetById(int id)
        {
            // only admins can access other user records
            var currentUser = (User)HttpContext.Items["User"];

            if (currentUser == null)
            {
                return Unauthorized(new { message = "Unauthorized" });
            }

            if (id != currentUser.Id && currentUser.Role != Role.Admin) // user can only access their own User info
            {
#if DEBUG
                _logger.LogError($"[!] GetById({id}) " +
                    $" @ {DateTime.UtcNow.ToLongTimeString()}" +
                    $"\n\t Unauthorized");
#endif
                return Unauthorized(new { message = "Unauthorized" });
            }

            var user = _userService.GetById(id);

            if (user== null)
            {
#if DEBUG
                _logger.LogError($"[!] GetById({id}) > failed to find user" +
                    $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

                return BadRequest(new { message = "User not found" });
            }

#if DEBUG
            _logger.LogInformation($"[+] GetById({id}) > user: " +
                $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

            return Ok(user);
        }
    }
}
