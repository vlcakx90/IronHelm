using Castle.Api.HostedFile;
using Castle.Interfaces;
using Castle.Models.Commander;
using Castle.Models.HostedFile;
using Microsoft.AspNetCore.Mvc;

namespace Castle.Controllers
{
    [ApiController]
    [Route("hostedfiles")]
    public class HostedFileController : ControllerBase
    {
        private readonly IHostedFileService _fileService;
        private readonly ICommanderService _commanderService;

        private readonly IRequestHeader _requestHeader;
        private readonly ILogger<HostedFileController> _logger;

        public HostedFileController(IHostedFileService hostedFileService, ICommanderService commanderService, ILogger<HostedFileController> logger, IRequestHeader requestHeader)
        {
            _fileService = hostedFileService;
            _commanderService = commanderService;
            _logger = logger;            
            _requestHeader = requestHeader;
        }

        [HttpGet(Name = "GetHostedFiles")]
        public async Task<ActionResult<IEnumerable<HostedFileResponse>>> GetHostedFiles()
        {
            var fileDaos = await _fileService.Get();
            var files = fileDaos.Select(f => (HostedFileResponse)f);

#if DEBUG            
            _logger.LogInformation($"[+] GetHostedFiles() > user: " +
                $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

            return Ok(files);
        }

        [HttpGet("{id}", Name = "GetHostedFile")]
        public async Task<ActionResult<HostedFileResponse>> GetHostedFile(string id)
        {
            var file = await _fileService.Get(id);
            if (file == null)
            {
#if DEBUG
                _logger.LogError($"[!] GetHostedFile({id}) > user: " +
                    $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                    $" @ {DateTime.UtcNow.ToLongTimeString()}" +
                    $"\n\t File not found");
#endif
                return NotFound();
            }

#if DEBUG            
            _logger.LogInformation($"[+] GetHostedFile({id}) > user: " +
                $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

            return Ok((HostedFileResponse)file);
        }

        [HttpPost(Name = "HostFile")]
        public async Task<ActionResult<HostedFileResponse>> HostFile([FromBody] HostFileRequest request)
        {
            var commander = _commanderService.GetCommander(request.Commander);
            if (commander == null)
            {
#if DEBUG
                _logger.LogError($"[!] HostFile(name:{request.Filename}) > user: " +
                    $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                    $" @ {DateTime.UtcNow.ToLongTimeString()}" +
                    $"\n\t Commander not found");
#endif
                return NotFound();
            }
            
            var filename = request.Uri.Split('/').Last();
            var fullPath = Path.Combine(((HttpCommander)commander).FileServerDirectory, filename);
            await System.IO.File.WriteAllBytesAsync(fullPath, request.Bytes);

            var hostedFile = (HostedFile)request;

            await _fileService.Add(hostedFile);

#if DEBUG            
            _logger.LogInformation($"[+] HostFile(name:{request.Filename}) > user: " +
                $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

            return Ok((HostedFileResponse)hostedFile);
        }

        [HttpDelete("{id}", Name = "DeleteHostedFile")]
        public async Task<ActionResult> DeleteHostedFile(string id)
        {
            var hostedFile = await _fileService.Get(id);

            if (hostedFile == null)
            {
#if DEBUG
                _logger.LogError($"[!] DeleteHostedFile({id}) > user: " +
                    $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                    $" @ {DateTime.UtcNow.ToLongTimeString()}" +
                    $"\n\t File not found");
#endif
                return NotFound("File not found");
            }

            var commander = _commanderService.GetCommander(hostedFile.Commander);
            if (commander == null)
            {
#if DEBUG
                _logger.LogError($"[!] DeleteHostedFile({id}) > user: " +
                    $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                    $" @ {DateTime.UtcNow.ToLongTimeString()}" +
                    $"\n\t Commander not found");
#endif
                return NotFound("Commander not found");
            }

            //var fileName = hostedFile.Uri.Split("/").Last();
            var fileName = hostedFile.Filename;
            var filePath = Path.Combine(((HttpCommander)commander).FileServerDirectory, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            await _fileService.Delete(hostedFile);

#if DEBUG            
            _logger.LogInformation($"[+] DeleteHostedFile({id}) > user: " +
                $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

            return Ok();
        }
    }
}
