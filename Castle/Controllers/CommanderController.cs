using Castle.Api.Commander;
using Castle.Auth;
using Castle.Interfaces;
using Castle.Models.Commander;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Castle.Controllers
{
    [ApiController]
    [Route("commander")]
    public class CommanderController : ControllerBase
    {
        private readonly ICommanderService _commanderService;
        private readonly IKnightService _soldierService;
        private readonly IRavenService _ravenService;
        private readonly IC2ProfileService _c2ProfileService;

        private readonly IRequestHeader _requestHeader;
        private readonly ILogger<CommanderController> _logger;        

        public CommanderController(ICommanderService commanderService, IKnightService soldierService, IRequestHeader requestHeader, ILogger<CommanderController> logger, IRavenService ravenService, IC2ProfileService c2ProfileService)
        {
            _commanderService = commanderService;
            _soldierService = soldierService;
            _requestHeader = requestHeader;
            _logger = logger;
            _ravenService = ravenService;
            _c2ProfileService = c2ProfileService;
        }

        [HttpGet(Name = "GetCommanders")]
        public ActionResult<IEnumerable<Commander>> GetCommanders()
        {
            var listeners = _commanderService.GetCommanders();

#if DEBUG
            _logger.LogInformation($"[+] GetCommanders() > user: " +
                $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif
            return Ok(listeners);
        }

        [HttpGet("{name}", Name = "GetCommander")]
        public ActionResult<Commander> GetCommander(string name)
        {
            var listener = _commanderService.GetCommander(name);
            if (listener is null)
            {
#if DEBUG
                _logger.LogError($"[!] GetCommander({name}) > user: " +
                    $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                    $" @ {DateTime.UtcNow.ToLongTimeString()}" +
                    $"\n\t Commander not found");
#endif
                return NotFound();
            }

#if DEBUG
            _logger.LogInformation($"[+] GetCommander({name}) > user: " +
                $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif
            return Ok(listener);
        }

        [HttpPost(Name = "StartCommander")]
        public async Task<ActionResult<Commander>> StartCommander([FromBody] StartHttpCommanderRequest request)
        {
            Commander listener = new HttpCommander(request.Name, request.BindPort, request.Tls);
            string root;
            string path;
            if (_commanderService.CheckDuplicateCommander(listener))
            {
#if DEBUG
                _logger.LogError($"[!] StartCommander({request.Name}) > user: " +
                    $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                    $" @ {DateTime.UtcNow.ToLongTimeString()}" +
                    $"\n\t Commander already exists");
#endif
                return Conflict("Commander already exists!");
            }

            listener.Init(_soldierService, _ravenService, _c2ProfileService);
            listener.Start(); // LEAVE SYNCHRONOUS

            await _commanderService.AddCommander(listener);

            root = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            path = $"{root}/{listener.Name}";

#if DEBUG
            _logger.LogInformation($"[+] StartCommander({request.Name}) > user: " +
                $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

            return Created(path, listener);
        }

        [HttpDelete("{name}", Name = "StopCommander")]
        public async Task<IActionResult> StopCommander(string name)
        {
            var listener = _commanderService.GetCommander(name);
            if (listener is null)
            {
#if DEBUG
                _logger.LogError($"[!] StopCommander({name}) > user: " +
                    $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                    $" @ {DateTime.UtcNow.ToLongTimeString()}" +
                    $"\n\t Commander not found");
#endif
                return NotFound();
            }

            listener.Stop();
            await _commanderService.RemoveCommander(listener);

#if DEBUG
            _logger.LogInformation($"[+] StopCommander({name}) > user: " +
                $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif
            return Ok();
        }
    }
}
