using Castle.Api.C2Profile;
using Castle.Interfaces;
using Castle.Models.C2Profile;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace Castle.Controllers
{
    [ApiController]
    [Route("c2profile")]
    public class C2ProfileController : ControllerBase
    {
        private readonly IC2ProfileService _c2ProfileService;
        private readonly IRequestHeader _requestHeader;
        private readonly ILogger<C2ProfileController> _logger;

        public C2ProfileController(IC2ProfileService c2ProfileService, ILogger<C2ProfileController> logger, IRequestHeader requestHeader)
        {
            _c2ProfileService = c2ProfileService;
            _logger = logger;
            _requestHeader = requestHeader;
        }

        [HttpPost(Name = "SetProfile")]
        public async Task<ActionResult<C2Profile>> SetProfile(string name)
        {
            var profile = await _c2ProfileService.SetProfile(name);

            if (profile == null)
            {
#if DEBUG                
                _logger.LogError($"[!] SetProfile({name}) > user: " +
                    $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                    $" @ {DateTime.UtcNow.ToLongTimeString()}" +
                    $"\n\t Failed to set Profile");                
#endif
                return Conflict("Failed to set profile");
            }

#if DEBUG            
            _logger.LogInformation($"[+] SetProfile({name}) > user: " +
                $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

            return Ok(profile);
        }

        // get current profile
        [HttpGet(Name = "GetCurrentProfile")]
        public ActionResult<C2Profile> GetCurrentProfile()
        {
            var profile = _c2ProfileService.GetCurrentProfile();

#if DEBUG
            _logger.LogInformation($"[+] GetCurrentProfile() > user: " +
                $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

            return Ok(profile);
        }

        // get all
        [HttpGet("getprofiles", Name = "GetProfiles")]
        public async Task<ActionResult<IEnumerable<C2Profile>>> GetProfiles()
        {
            var profiles = await _c2ProfileService.GetProfiles();

#if DEBUG            
            _logger.LogInformation($"[+] GetProfiles() > user: " +
                $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

            return Ok(profiles);
        }

        // get by name
        [HttpGet("{name}", Name = "GetProfile")]
        public async Task<ActionResult<C2Profile>> GetProfile(string name)
        {
            var profile = await _c2ProfileService.GetProfile(name);

            if (profile == null)
            {
#if DEBUG
                _logger.LogError($"[!] GetProfile({name}) > user: " +
                    $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                    $" @ {DateTime.UtcNow.ToLongTimeString()}" +
                    $"\n\t Profile not found");
#endif
                return NotFound("Profile not found");
            }

#if DEBUG
            _logger.LogInformation($"[+] GetProfile({name}) > user: " +
                $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

            return Ok(profile);
        }

        // create
        [HttpPost("create", Name = "CreateProfile")]
        public async Task<ActionResult<C2Profile>> CreateProfile([FromBody] CreateC2ProfileRequest request)
        {
            C2Profile profile = new C2Profile
            {
                Name = request.Name,
                Http = new C2Profile.HttpProfile
                {
                    Sleep = request.Sleep,
                    Jitter = request.Jitter,
                    Passwd = request.Passwd,
                    GetPaths = request.GetPaths,
                    PostPaths = request.PostPaths,
                }
            };

            string filename = request.Name;
            //// check duplicate
            //if (!_c2ProfileService.CheckDuplicateProfile(filename))
            //{
            //    var result = await _c2ProfileService.CreateProfile(filename, profile);
            //    if (result)
            //    {
            //        return Ok(profile);
            //    }
            //}

            //return Conflict("Profile already exists (name collision)");

            // check duplicate
            if (_c2ProfileService.CheckDuplicateProfile(filename))
            {
#if DEBUG
                _logger.LogError($"[!] CreateProfile({request.Name}) > user: " +
                    $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                    $" @ {DateTime.UtcNow.ToLongTimeString()}" +
                    $"\n\t Profile already exists (name collision)");
#endif
                return Conflict("Profile already exists (name collision)");
            }

            var result = await _c2ProfileService.CreateProfile(filename, profile);
            if (!result)
            {
#if DEBUG
                _logger.LogError($"[!] CreateProfile({request.Name}) > user: " +
                    $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                    $" @ {DateTime.UtcNow.ToLongTimeString()}" +
                    $"\n\t Failed to Create Profile)");
#endif
                return Conflict("Failed to Create Profile");
            }

#if DEBUG
            _logger.LogInformation($"[+] CreateProfile({request.Name}) > user: " +
                $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

            return Ok(profile);
        }

        // update
        [HttpPost("update", Name = "UpdateProfile")]
        public async Task<ActionResult<C2Profile>> UpdateProfile([FromBody] CreateC2ProfileRequest request)
        {
            C2Profile profile = new C2Profile
            {
                Name = request.Name,
                Http = new C2Profile.HttpProfile
                {
                    Sleep = request.Sleep,
                    Jitter = request.Jitter,
                    Passwd = request.Passwd,
                    GetPaths = request.GetPaths,
                    PostPaths = request.PostPaths,
                }
            };

            string filename = request.Name;



            var result = await _c2ProfileService.CreateProfile(filename, profile);
            if (!result)
            {
#if DEBUG
                _logger.LogError($"[!] UpdateProfile({request.Name}) > user: " +
                    $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                    $" @ {DateTime.UtcNow.ToLongTimeString()}" +
                    $"\n\t Profile already exists (name collision)");
#endif
                return Conflict("Profile already exists (name collision)");
            }

#if DEBUG
            _logger.LogInformation($"[+] UpdateProfile({request.Name}) > user: " +
                $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif
            return Ok(profile);
        }

        // delete
        [HttpDelete("{name}", Name = "DeleteProfile")]
        public ActionResult DeleteProfile(string name)
        {
            var result = _c2ProfileService.DeleteProfile(name);


            if (!result)
            {
#if DEBUG
                _logger.LogError($"[!] DeleteProfile({name}) > user: " +
                    $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                    $" @ {DateTime.UtcNow.ToLongTimeString()}" +
                    $"\n\t Failed to delete");
#endif
                return Conflict("Failed to delete");
            }

#if DEBUG
            _logger.LogInformation($"[+] DeleteProfile({name}) > user: " +
                $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif
            return Ok();
        }
    }
}
